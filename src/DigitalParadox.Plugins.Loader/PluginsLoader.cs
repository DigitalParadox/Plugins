using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using DigitalParadox.Utilities.AssemblyLoader;
using System.IO;
using DigitalParadox.Logging;
using Machine.Specifications.Annotations;

//TODO : add System Configuration Support to aid/filter  discovery locations
namespace DigitalParadox.Plugins.Loader
{
    public static class PluginsLoader
    {
        internal static IEnumerable<Type> GetPluginCollection<T>(this IEnumerable<Assembly> assemblies)
            where T : IPlugin
        {
            return assemblies.SelectMany(a => GetPluginCollection<T>(a).Where(q => !q.IsInterface));
        }

        public static ILog Log { get; set; }

        internal static IEnumerable<Type> GetPluginCollection<T>(this Assembly assembly)
            where T : IPlugin
        {
            return AssemblyLoader.GetTypes<T>();
        }

        public static IDictionary<string, Type> GetPlugins<T>(DirectoryInfo di = null)
            where T : IPlugin
        {
            IEnumerable<Type> types = di == null ? 
            
                AssemblyLoader.GetAppDomainAssemblies<T>().GetPluginCollection<T>() :
                AssemblyLoader.GetAssemblies<T>(di).GetPluginCollection<T>();
                

            var dictionary = new Dictionary<string, Type>();

            foreach (var type in types)
            {
                var pInfo = type.GetCustomAttribute(typeof(PluginInfoAttribute)) as PluginInfoAttribute;
                if (!string.IsNullOrWhiteSpace(pInfo?.Name))
                    dictionary.Add(pInfo.Name, type);

                dictionary.Add(type.FullName, type);
                WriteLog($"Loaded Type: { type.FullName } from { type.Assembly.Location }", LogLevel.Verbose );
            }
            WriteLog($"Loaded {dictionary.Count } Types");

            return dictionary;
        }

        public static IDictionary<string, Type> GetPlugins<T>(this Assembly assembly)
            where T : IPlugin
        {
            var dictionary = new Dictionary<string, Type>();
            foreach (var type in assembly.GetPluginCollection<T>())
            {
                var pInfo = type.GetCustomAttribute(typeof(PluginInfoAttribute)) as PluginInfoAttribute;
                
                if (!string.IsNullOrWhiteSpace(pInfo?.Name))
                    dictionary.Add(pInfo.Name, type);

                dictionary.Add(type.FullName, type);
            }
            return dictionary;
        }
        
        public static IDictionary<string, Type> GetPlugins<T>(this IEnumerable<Assembly> assemblies)
            where T : IPlugin
        {
            return null;
        }

        private static void WriteLog([NotNull] string message, LogLevel level = LogLevel.Information)
        {
            if (Log != null)
            {
                switch (level)
                {
                    case LogLevel.Error:
                        Log.WriteError(message);
                        break;
                    case LogLevel.Verbose:
                        Log.WriteVerbose(message);
                        break;
                    case LogLevel.Warning:
                        Log.WriteWarning(message);
                        break;
                    case LogLevel.Information:
                        Log.WriteInformation(message);
                        break;

                }
            }
        }

        enum LogLevel
        {
            Error, 
            Verbose, 
            Warning, 
            Information
        }
    }
}
