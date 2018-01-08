﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Stream.Upgrade.Properties {
    using System;
    
    
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
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Stream.Upgrade.Properties.Resources", typeof(Resources).Assembly);
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
        ///   Looks up a localized string similar to SELECT COUNT(*)
        ///FROM &quot;STR_Account&quot;
        ///WHERE &quot;LastImportReference&quot; IS NOT NULL;.
        /// </summary>
        internal static string CountCasesWithLastImportReference {
            get {
                return ResourceManager.GetString("CountCasesWithLastImportReference", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 
        ///SELECT COUNT(CSE.&quot;CaseId&quot;)
        ///FROM &quot;STR_Case&quot; CSE
        ///LEFT JOIN &quot;STR_Account&quot; ACC ON CSE.&quot;AccountId&quot; = ACC.&quot;AccountId&quot;
        ///WHERE CSE.&quot;BranchId&quot; IS NULL
        ///AND ACC.&quot;BranchId&quot; IS NOT NULL.
        /// </summary>
        internal static string CountCasesWithoutBranch {
            get {
                return ResourceManager.GetString("CountCasesWithoutBranch", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 
        ///	SELECT COUNT(DC.&quot;DebtorContactId&quot;)
        ///	FROM &quot;STR_DebtorContact&quot; DC
        ///	LEFT JOIN &quot;Contact&quot; CT ON DC.&quot;ContactId&quot; = CT.&quot;ContactId&quot;
        ///	WHERE DC.&quot;Value&quot; IS NULL.
        /// </summary>
        internal static string CountCasesWithoutContact {
            get {
                return ResourceManager.GetString("CountCasesWithoutContact", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT COUNT(CSE.&quot;CaseId&quot;)
        ///FROM &quot;STR_Case&quot; CSE
        ///LEFT JOIN &quot;STR_Account&quot; ACC ON CSE.&quot;AccountId&quot; = ACC.&quot;AccountId&quot;
        ///WHERE CSE.&quot;DebtorId&quot; IS NULL
        ///AND ACC.&quot;DebtorId&quot; IS NOT NULL
        ///LIMIT 10000.
        /// </summary>
        internal static string CountCasesWithoutDebtor {
            get {
                return ResourceManager.GetString("CountCasesWithoutDebtor", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT COUNT(CSE.&quot;CaseId&quot;)
        ///FROM &quot;STR_Case&quot; CSE
        ///LEFT JOIN &quot;STR_Account&quot; ACC ON CSE.&quot;AccountId&quot; = ACC.&quot;AccountId&quot;
        ///WHERE CSE.&quot;HostId&quot; IS NULL
        ///AND ACC.&quot;HostId&quot; IS NOT NULL.
        /// </summary>
        internal static string CountCasesWithoutHost {
            get {
                return ResourceManager.GetString("CountCasesWithoutHost", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT COUNT(CSE.&quot;CaseId&quot;)
        ///FROM &quot;STR_Case&quot; CSE
        ///LEFT JOIN &quot;BRN_Branch&quot; BRN ON CSE.&quot;BranchId&quot; = BRN.&quot;BranchId&quot;
        ///LEFT JOIN &quot;STR_Debtor&quot; DBT ON CSE.&quot;DebtorId&quot; = DBT.&quot;DebtorId&quot;
        ///WHERE CSE.&quot;Reference&quot; IS NULL.
        /// </summary>
        internal static string CountCasesWithoutReference {
            get {
                return ResourceManager.GetString("CountCasesWithoutReference", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 
        ///SELECT COUNT(&quot;NoteId&quot;)
        ///FROM &quot;STR_AccountNote&quot;.
        /// </summary>
        internal static string CountNotes {
            get {
                return ResourceManager.GetString("CountNotes", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT COUNT(&quot;NoteId&quot;)
        ///FROM &quot;STR_Note&quot;
        ///WHERE &quot;AccountNoteTypeId&quot; = 1.
        /// </summary>
        internal static string CountTransactionNotesRemaining {
            get {
                return ResourceManager.GetString("CountTransactionNotesRemaining", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to DELETE
        ///FROM &quot;STR_Note&quot;
        ///WHERE &quot;NoteId&quot; IN (
        ///	SELECT &quot;NoteId&quot;
        ///	FROM &quot;STR_Note&quot;
        ///	WHERE &quot;AccountNoteTypeId&quot; = 1
        ///	ORDER BY &quot;NoteId&quot; DESC
        ///	LIMIT 10000).
        /// </summary>
        internal static string DeleteTransactionNotes {
            get {
                return ResourceManager.GetString("DeleteTransactionNotes", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} {1}: remaining records - {2}.
        /// </summary>
        internal static string Program_Upgrade__0___remaining_records____1_ {
            get {
                return ResourceManager.GetString("Program_Upgrade__0___remaining_records____1_", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to UPDATE &quot;STR_Account&quot;
        ///SET &quot;LastImportReference&quot; = NULL
        ///WHERE &quot;LastImportReference&quot; IS NOT NULL
        ///LIMIT 10000;.
        /// </summary>
        internal static string UpdateCasesWithLastImportReference {
            get {
                return ResourceManager.GetString("UpdateCasesWithLastImportReference", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 
        ///UPDATE &quot;STR_Case&quot; CSE
        ///SET &quot;BranchId&quot; = UP.&quot;BranchId&quot;
        ///FROM (
        ///	SELECT CSE.&quot;CaseId&quot;, ACC.&quot;BranchId&quot;
        ///	FROM &quot;STR_Case&quot; CSE
        ///	LEFT JOIN &quot;STR_Account&quot; ACC ON CSE.&quot;AccountId&quot; = ACC.&quot;AccountId&quot;
        ///	WHERE CSE.&quot;BranchId&quot; IS NULL
        ///	AND ACC.&quot;BranchId&quot; IS NOT NULL
        ///	LIMIT 10000) UP
        ///WHERE CSE.&quot;CaseId&quot; = UP.&quot;CaseId&quot;;.
        /// </summary>
        internal static string UpdateCasesWithoutBranch {
            get {
                return ResourceManager.GetString("UpdateCasesWithoutBranch", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 
        ///UPDATE &quot;STR_DebtorContact&quot; DB
        ///SET &quot;Value&quot; = UP.&quot;Value&quot;, &quot;ContactTypeId&quot; = UP.&quot;ContactTypeId&quot;, &quot;IsActive&quot; = UP.&quot;IsActive&quot;
        ///FROM (
        ///	SELECT DC.&quot;DebtorContactId&quot;, CT.&quot;Value&quot;, CT.&quot;ContactTypeId&quot;, CT.&quot;IsActive&quot;
        ///	FROM &quot;STR_DebtorContact&quot; DC
        ///	LEFT JOIN &quot;Contact&quot; CT ON DC.&quot;ContactId&quot; = CT.&quot;ContactId&quot;
        ///	WHERE DC.&quot;Value&quot; IS NULL
        ///	LIMIT 10000) UP
        ///WHERE DB.&quot;DebtorContactId&quot; = UP.&quot;DebtorContactId&quot;;.
        /// </summary>
        internal static string UpdateCasesWithoutContact {
            get {
                return ResourceManager.GetString("UpdateCasesWithoutContact", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to UPDATE &quot;STR_Case&quot; CSE
        ///SET &quot;DebtorId&quot; = UP.&quot;DebtorId&quot;
        ///FROM (
        ///	SELECT CSE.&quot;CaseId&quot;, ACC.&quot;DebtorId&quot;
        ///	FROM &quot;STR_Case&quot; CSE
        ///	LEFT JOIN &quot;STR_Account&quot; ACC ON CSE.&quot;AccountId&quot; = ACC.&quot;AccountId&quot;
        ///	WHERE CSE.&quot;DebtorId&quot; IS NULL
        ///	AND ACC.&quot;DebtorId&quot; IS NOT NULL
        ///	LIMIT 10000) UP
        ///WHERE CSE.&quot;CaseId&quot; = UP.&quot;CaseId&quot;;.
        /// </summary>
        internal static string UpdateCasesWithoutDebtor {
            get {
                return ResourceManager.GetString("UpdateCasesWithoutDebtor", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 
        ///UPDATE &quot;STR_Case&quot; CSE
        ///SET &quot;HostId&quot; = UP.&quot;HostId&quot;
        ///FROM (
        ///	SELECT CSE.&quot;CaseId&quot;, ACC.&quot;HostId&quot;
        ///	FROM &quot;STR_Case&quot; CSE
        ///	LEFT JOIN &quot;STR_Account&quot; ACC ON CSE.&quot;AccountId&quot; = ACC.&quot;AccountId&quot;
        ///	WHERE CSE.&quot;HostId&quot; IS NULL
        ///	AND ACC.&quot;HostId&quot; IS NOT NULL
        ///	LIMIT 10000) UP
        ///WHERE CSE.&quot;CaseId&quot; = UP.&quot;CaseId&quot;;.
        /// </summary>
        internal static string UpdateCasesWithoutHost {
            get {
                return ResourceManager.GetString("UpdateCasesWithoutHost", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to UPDATE &quot;STR_Case&quot; CSE
        ///SET &quot;Reference&quot; = UP.&quot;CaseReference&quot;
        ///FROM (
        ///	SELECT CSE.&quot;CaseId&quot;, CSE.&quot;Reference&quot;, BRN.&quot;LegacyBranchNum&quot;, DBT.&quot;Reference&quot;, BRN.&quot;LegacyBranchNum&quot; || &apos;X&apos; || DBT.&quot;Reference&quot; AS &quot;CaseReference&quot;
        ///	FROM &quot;STR_Case&quot; CSE
        ///	LEFT JOIN &quot;BRN_Branch&quot; BRN ON CSE.&quot;BranchId&quot; = BRN.&quot;BranchId&quot;
        ///	LEFT JOIN &quot;STR_Debtor&quot; DBT ON CSE.&quot;DebtorId&quot; = DBT.&quot;DebtorId&quot;
        ///	WHERE CSE.&quot;Reference&quot; IS NULL
        ///	ORDER BY 1 desc
        ///	LIMIT 1000) UP
        ///WHERE CSE.&quot;CaseId&quot; = UP.&quot;CaseId&quot;;
        ///
        ///.
        /// </summary>
        internal static string UpdateCasesWithoutReference {
            get {
                return ResourceManager.GetString("UpdateCasesWithoutReference", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to INSERT INTO &quot;STR_Note&quot; (&quot;Note&quot;, &quot;CaseId&quot;, &quot;AccountNoteTypeId&quot;, &quot;CreateUserId&quot;, &quot;CreateDate&quot;)
        ///SELECT NTE.&quot;Note&quot;, CSE.&quot;CaseId&quot;, NT.&quot;AccountNoteTypeId&quot;, NT.&quot;CreateUserId&quot;, NT.&quot;CreateDate&quot;
        ///FROM &quot;STR_AccountNote&quot; NT
        ///INNER JOIN &quot;STR_Case&quot; CSE ON NT.&quot;AccountId&quot; = CSE.&quot;AccountId&quot;
        ///INNER JOIN &quot;NTE_Note&quot; NTE ON NT.&quot;NoteId&quot; = NTE.&quot;NoteId&quot;
        ///ORDER BY NT.&quot;NoteId&quot;
        ///LIMIT 100000;
        ///
        ///DELETE FROM &quot;NTE_Note&quot; WHERE &quot;ParentNoteId&quot; IN (
        ///SELECT &quot;NoteId&quot;
        ///FROM &quot;STR_AccountNote&quot;
        ///ORDER BY &quot;NoteId&quot;
        ///LIMIT 100000);
        ///
        ///DELETE FROM &quot;NTE_Note&quot; WH [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string UpdateNote {
            get {
                return ResourceManager.GetString("UpdateNote", resourceCulture);
            }
        }
    }
}
