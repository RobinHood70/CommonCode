﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace RobinHood70.CommonCode.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("RobinHood70.CommonCode.Properties.Resources", typeof(Resources).Assembly);
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
        ///   Looks up a localized string similar to Array size is less than the dimensions specified..
        /// </summary>
        internal static string ArrayTooSmall {
            get {
                return ResourceManager.GetString("ArrayTooSmall", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The collection {0} must contain values; it cannot be null or empty..
        /// </summary>
        internal static string CollectionEmpty {
            get {
                return ResourceManager.GetString("CollectionEmpty", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The collection {0} cannot contain values that are null or only whitespace..
        /// </summary>
        internal static string CollectionInvalid {
            get {
                return ResourceManager.GetString("CollectionInvalid", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Multiple items with the same key were found in the provided collection. Key: {0}.
        /// </summary>
        internal static string DuplicateKeyInItems {
            get {
                return ResourceManager.GetString("DuplicateKeyInItems", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Array bounds must be greater than zero..
        /// </summary>
        internal static string InvalidArrayBounds {
            get {
                return ResourceManager.GetString("InvalidArrayBounds", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The four-character code is not four characters..
        /// </summary>
        internal static string InvalidFourCC {
            get {
                return ResourceManager.GetString("InvalidFourCC", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The LinkedListNode specified does not belong to a list..
        /// </summary>
        internal static string NoNodeList {
            get {
                return ResourceManager.GetString("NoNodeList", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} is not at end of data..
        /// </summary>
        internal static string NotAtEnd {
            get {
                return ResourceManager.GetString("NotAtEnd", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The {0} parameter passed to {1} could not be cast to {2}, as it&apos;s type was {3}..
        /// </summary>
        internal static string ParameterInvalidCast {
            get {
                return ResourceManager.GetString("ParameterInvalidCast", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The parameter {0} cannot be null or only whitespace..
        /// </summary>
        internal static string StringInvalid {
            get {
                return ResourceManager.GetString("StringInvalid", resourceCulture);
            }
        }
    }
}
