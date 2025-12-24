using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OutwardGameSettings.Managers.Testing
{
    public static class DynamicDebugger
    {
        // Keep track of patched methods to avoid double patching
        private static HashSet<string> patchedMethods = new HashSet<string>();
        private static Harmony harmonyInstance;

        public static void Init()
        {
            harmonyInstance = new Harmony("gymmed.dynamicdebugger");

            // Subscribe to all Unity log messages
            Application.logMessageReceived += OnLogMessage;
        }

        private static void OnLogMessage(string condition, string stackTrace, LogType type)
        {
            if (type != LogType.Exception) return;

            LogWithPrefix($"{condition}\n{stackTrace}", "GLOBAL EXCEPTION");

            var firstLine = stackTrace?.Split('\n')[0];
            if (firstLine == null) return;

            string className = null;
            string methodName = null;

            if (firstLine.Contains("(wrapper dynamic-method)"))
            {
                // Extract real type and method
                // Example: "(wrapper dynamic-method) AttackSkill.DMD<AttackSkill::OwnerHasAllRequiredItems>(AttackSkill,bool)"
                int start = firstLine.IndexOf(")") + 2; // skip past ") "
                int dmdIndex = firstLine.IndexOf(".DMD<", start);
                if (dmdIndex > 0)
                {
                    className = firstLine.Substring(start, dmdIndex - start);
                    int methodStart = firstLine.IndexOf("::", dmdIndex) + 2;
                    int methodEnd = firstLine.IndexOf(">", methodStart);
                    methodName = firstLine.Substring(methodStart, methodEnd - methodStart);
                }
            }
            else
            {
                // Example: OutwardGameSettings.Managers.SeasonsManager.TryAddSeason (...)
                string[] parts = firstLine.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length >= 2)
                {
                    // Method is last segment, minus parameters
                    string rawLast = parts[parts.Length - 1];
                    methodName = rawLast.Split('(')[0].Trim();

                    // Class is everything before last segment
                    className = string.Join(".", parts, 0, parts.Length - 1);
                }
            }

            if (!string.IsNullOrEmpty(className) && !string.IsNullOrEmpty(methodName))
            {
                string key = $"{className}.{methodName}";
                if (!patchedMethods.Contains(key))
                {
                    LogDebugPatch($"Applying dynamic patch to {key}");
                    TryPatchMethod(className, methodName);
                    patchedMethods.Add(key);
                }
            }
        }

        private static void TryPatchMethod(string className, string methodName)
        {
            try
            {
                // Find the type
                Type targetType = Type.GetType(className) ?? 
                                  AppDomain.CurrentDomain.GetAssemblies()
                                    .SelectMany(a => a.GetTypes())
                                    .FirstOrDefault(t => t.Name == className);

                if (targetType == null)
                {
                    LogDebugPatch($"Could not find type {className}");
                    return;
                }

                MethodInfo targetMethod = targetType.GetMethod(
                    methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                if (targetMethod == null)
                {
                    LogDebugPatch($"Could not find method {methodName} in {className}");
                    return;
                }

                MethodInfo exceptionPostfix = typeof(DynamicDebugger).GetMethod(
                    nameof(DebugPostfix), BindingFlags.Static | BindingFlags.NonPublic);

                var harmonyMethod = new HarmonyMethod(exceptionPostfix)
                {
                    priority = Priority.Last,     // Run after other postfixes
                    after = new string[] { "*" },  // Run after any other mod
                };

                harmonyInstance.Patch(targetMethod, finalizer: harmonyMethod);

                LogDebugPatch($"Successfully patched {className}.{methodName}");
            }
            catch (Exception e)
            {
                LogWithPrefix(e.Message, "DEBUG PATCH ERROR");
            }
        }

        private static void DebugPostfix(Exception __exception, object __instance, MethodBase __originalMethod)
        {
            if (__instance == null || __exception == null) return;

            Type type = __instance.GetType();
            string methodName = __originalMethod?.Name ?? "Unknown";
            LogDebug($"Exception in {type.Name}.{methodName}: {__exception.Message}", "DEBUG CRASH");

            AnalyzeNullReferenceException(__instance, __exception);
            DebugPrefixFields(__instance);
            DebugPrefixProperties(__instance);
        }

        private static string GetCurrentMethodName()
        {
            var st = new StackTrace(new StackFrame(1));
            return st.GetFrame(0)?.GetMethod()?.Name ?? "Unknown";
        }

        // Prefix to log all fields of the instance
        private static void DebugPrefixFields(object __instance)
        {
            if (__instance == null) return;

            Type type = __instance.GetType();
            LogField($"{type.Name} instance:");

            foreach (var field in type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
            {
                try
                {
                    object value = field.GetValue(__instance);
                    LogField($"{field.Name} = {value}");
                }
                catch (Exception e)
                {
                    LogField($"{field.Name} = <FAILED: {e.Message}>");
                }
            }
        }

        private static void DebugPrefixProperties(object __instance)
        {
            if (__instance == null) return;

            Type type = __instance.GetType();
            LogProperty($"{type.Name} instance:");

            // Log properties
            foreach (var prop in type.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
            {
                if (prop.GetIndexParameters().Length > 0) continue; // skip indexers
                try
                {
                    object value = prop.GetValue(__instance);
                    LogProperty($"{prop.Name} = {value}");
                }
                catch (Exception e)
                {
                    LogProperty($"{prop.Name} = <FAILED: {e.Message}>");
                }
            }
        }

        private static void AnalyzeNullReferenceException(object __instance, Exception exception)
        {
            if (!(exception is NullReferenceException)) return;

            Type type = __instance.GetType();
            List<string> nullFields = new List<string>();
            List<string> nullProperties = new List<string>();

            // Find all null fields
            foreach (var field in type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
            {
                try
                {
                    object value = field.GetValue(__instance);
                    if (value == null && !field.FieldType.IsValueType)
                    {
                        nullFields.Add(field.Name);
                    }
                }
                catch { }
            }

            // Find all null properties
            foreach (var prop in type.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
            {
                if (prop.GetIndexParameters().Length > 0) continue;
                if (!prop.CanRead) continue;
                
                try
                {
                    object value = prop.GetValue(__instance);
                    if (value == null && !prop.PropertyType.IsValueType)
                    {
                        nullProperties.Add(prop.Name);
                    }
                }
                catch { }
            }

            // Report findings
            if (nullFields.Count > 0 || nullProperties.Count > 0)
            {
                LogDebug("=== NULL REFERENCE ANALYSIS ===", "ANALYSIS");
                LogDebug($"Found {nullFields.Count} null fields and {nullProperties.Count} null properties", "ANALYSIS");
                
                if (nullFields.Count > 0)
                {
                    LogDebug("Null fields (potential causes):", "ANALYSIS");
                    foreach (var field in nullFields)
                    {
                        LogDebug($"  - {field}", "ANALYSIS");
                    }
                }

                if (nullProperties.Count > 0)
                {
                    LogDebug("Null properties (potential causes):", "ANALYSIS");
                    foreach (var prop in nullProperties)
                    {
                        LogDebug($"  - {prop}", "ANALYSIS");
                    }
                }

                LogDebug("=== LIKELY CAUSES ===", "ANALYSIS");
                LogDebug("The NullReferenceException likely occurred when accessing one of the above null members.", "ANALYSIS");
                LogDebug("Common patterns:", "ANALYSIS");
                LogDebug("  1. Calling a method on a null object", "ANALYSIS");
                LogDebug("  2. Accessing a property/field of a null object", "ANALYSIS");
                LogDebug("  3. Indexing into a null collection", "ANALYSIS");
            }
            else
            {
                LogDebug("No obviously null fields/properties found. The null reference may be:", "ANALYSIS");
                LogDebug("  - A local variable in the method", "ANALYSIS");
                LogDebug("  - A method parameter", "ANALYSIS");
                LogDebug("  - A temporary result from another call", "ANALYSIS");
            }
        }

        private static void LogField(string message = "")
        {
            LogDebug(message, "FIELDS");
        }

        private static void LogProperty(string message = "")
        {
            LogDebug(message, "PROPERTIES");
        }

        private static void LogDebugPatch(string message = "")
        {
            LogWithPrefix(message, "DEBUG PATCH");
        }

        private static void LogDebug(string message = "", params string[] prefixes)
        {
            string[] allPrefixes = new string[1 + prefixes.Length];
            allPrefixes[0] = "DEBUG";
            Array.Copy(prefixes, 0, allPrefixes, 1, prefixes.Length);

            LogWithPrefix(message, allPrefixes);
        }

        private static void LogWithPrefix(string message = "", params string[] prefixes)
        {
            string outputPrefix = "";

            foreach(string prefix in prefixes)
            {
                outputPrefix += $"[{prefix}]";
            }
            OutwardGameSettings.LogMessage($"{outputPrefix} {message}");
        }
    }
}
