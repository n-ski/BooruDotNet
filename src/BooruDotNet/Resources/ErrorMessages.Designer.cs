﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BooruDotNet.Resources {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class ErrorMessages {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal ErrorMessages() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("BooruDotNet.Resources.ErrorMessages", typeof(ErrorMessages).Assembly);
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
        ///   Looks up a localized string similar to An error has occured:.
        /// </summary>
        internal static string ExceptionStackHeader {
            get {
                return ResourceManager.GetString("ExceptionStackHeader", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid post type. Expected: &apos;{0}&apos;, actual: &apos;{1}&apos;..
        /// </summary>
        internal static string PostConverterInvalidTypeFormat {
            get {
                return ResourceManager.GetString("PostConverterInvalidTypeFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Couldn&apos;t retrieve a post with the hash &apos;{0}&apos;.
        /// </summary>
        internal static string PostInvalidHashFormat {
            get {
                return ResourceManager.GetString("PostInvalidHashFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Couldn&apos;t retrieve a post with the ID &apos;{0}&apos;.
        /// </summary>
        internal static string PostInvalidIdFormat {
            get {
                return ResourceManager.GetString("PostInvalidIdFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Couldn&apos;t retrieve a tag with the name &apos;{0}&apos;.
        /// </summary>
        internal static string TagInvalidNameFormat {
            get {
                return ResourceManager.GetString("TagInvalidNameFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to URI was not absolute..
        /// </summary>
        internal static string UriIsNotAbsolute {
            get {
                return ResourceManager.GetString("UriIsNotAbsolute", resourceCulture);
            }
        }
    }
}
