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
    internal class RequestUris {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal RequestUris() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("BooruDotNet.Resources.RequestUris", typeof(RequestUris).Assembly);
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
        ///   Looks up a localized string similar to https://danbooru.donmai.us/posts.json?limit=1&amp;md5={0}.
        /// </summary>
        internal static string DanbooruPostHash_Format {
            get {
                return ResourceManager.GetString("DanbooruPostHash_Format", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to https://danbooru.donmai.us/posts/{0}.json.
        /// </summary>
        internal static string DanbooruPostId_Format {
            get {
                return ResourceManager.GetString("DanbooruPostId_Format", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to https://danbooru.donmai.us/tags.json?search[name]={0}.
        /// </summary>
        internal static string DanbooruTagName_Format {
            get {
                return ResourceManager.GetString("DanbooruTagName_Format", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to http://gelbooru.com/index.php?page=post&amp;s=list&amp;md5={0}.
        /// </summary>
        internal static string GelbooruPostHash_Format {
            get {
                return ResourceManager.GetString("GelbooruPostHash_Format", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to https://gelbooru.com/index.php?page=dapi&amp;s=post&amp;q=index&amp;id={0}&amp;json=1.
        /// </summary>
        internal static string GelbooruPostId_Format {
            get {
                return ResourceManager.GetString("GelbooruPostId_Format", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to https://gelbooru.com/index.php?page=dapi&amp;s=tag&amp;q=index&amp;name={0}&amp;json=1.
        /// </summary>
        internal static string GelbooruTagName_Format {
            get {
                return ResourceManager.GetString("GelbooruTagName_Format", resourceCulture);
            }
        }
    }
}
