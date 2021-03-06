﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace System.Security.Cryptography.Hashing.Algorithms.Analyzers {
    using System;
    using System.Reflection;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class SystemSecurityCryptographyResources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal SystemSecurityCryptographyResources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Microsoft.NetCore.Analyzers.System.Security.Cryptography.Hashing.Algorithms.Analy" +
                            "zers.SystemSecurityCryptographyResources", typeof(SystemSecurityCryptographyResources).GetTypeInfo().Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Do not use insecure cryptographic algorithm MD5..
        /// </summary>
        internal static string DoNotUseMD5 {
            get {
                return ResourceManager.GetString("DoNotUseMD5", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This type implements MD5, a cryptographically insecure hashing function. Hash collisions are computationally feasible for the MD5 and HMACMD5 algorithms. Replace this usage with a SHA-2 family hash algorithm (SHA512, SHA384, SHA256)..
        /// </summary>
        internal static string DoNotUseMD5Description {
            get {
                return ResourceManager.GetString("DoNotUseMD5Description", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Do not use insecure cryptographic algorithm SHA1..
        /// </summary>
        internal static string DoNotUseSHA1 {
            get {
                return ResourceManager.GetString("DoNotUseSHA1", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This type implements SHA1, a cryptographically insecure hashing function. Hash collisions are computationally feasible for the SHA-1 and SHA-0 algorithms. Replace this usage with a SHA-2 family hash algorithm (SHA512, SHA384, SHA256)..
        /// </summary>
        internal static string DoNotUseSHA1Description {
            get {
                return ResourceManager.GetString("DoNotUseSHA1Description", resourceCulture);
            }
        }
    }
}
