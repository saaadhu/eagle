using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using EnvDTE;
using Microsoft.Win32;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Standard;

namespace SenthilKumarSelvaraj.Eagle
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    ///
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the 
    /// IVsPackage interface and uses the registration attributes defined in the framework to 
    /// register itself and its components with the shell.
    /// </summary>
    // This attribute tells the PkgDef creation utility (CreatePkgDef.exe) that this class is
    // a package.
    [PackageRegistration(UseManagedResourcesOnly = true)]
    // This attribute is used to register the informations needed to show the this package
    // in the Help/About dialog of Visual Studio.
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [Guid(GuidList.guidEaglePkgString)]
    [ProvideAutoLoad(UIContextGuids.SolutionExists)]
    public sealed class EaglePackage : Package
    {
        /// <summary>
        /// Default constructor of the package.
        /// Inside this method you can place any initialization code that does not require 
        /// any Visual Studio service because at this point the package object is created but 
        /// not sited yet inside Visual Studio environment. The place to do all the other 
        /// initialization is the Initialize method.
        /// </summary>
        public EaglePackage()
        {
            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", this.ToString()));
        }


        private DTE dte;
        /////////////////////////////////////////////////////////////////////////////
        // Overriden Package Implementation
        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initilaization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            Trace.WriteLine (string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this.ToString()));
            base.Initialize();

            dte = (DTE) Package.GetGlobalService(typeof (DTE));
            dte.Events.BuildEvents.OnBuildBegin += new _dispBuildEvents_OnBuildBeginEventHandler(BuildEvents_OnBuildBegin);
            dte.Events.BuildEvents.OnBuildDone += BuildEvents_OnBuildDone;

            InitializeTaskbarList();
        }

        private void BuildEvents_OnBuildDone(vsBuildScope scope, vsBuildAction action)
        {
            var mainWnd = new IntPtr(dte.MainWindow.HWnd);
            int failuresCount = dte.Solution.SolutionBuild.LastBuildInfo;

            taskbarList.SetProgressValue(mainWnd, 100, 100);
                
            taskbarList.SetProgressState(mainWnd,
                                         failuresCount == 0 ? TBPF.NORMAL : TBPF.ERROR);

        }

        private ITaskbarList3 taskbarList;
        private void InitializeTaskbarList()
        {
            ITaskbarList tempTaskbarList = null;
            try
            {
                tempTaskbarList = CLSID.CoCreateInstance<ITaskbarList>(CLSID.TaskbarList);
                tempTaskbarList.HrInit();

                // This QI will only work on Win7.
                taskbarList = tempTaskbarList as ITaskbarList3;

                tempTaskbarList = null;
            }
            finally
            {
                Utility.SafeRelease(ref tempTaskbarList);
            }
        }

        private void BuildEvents_OnBuildBegin(vsBuildScope scope, vsBuildAction action)
        {
            taskbarList.SetProgressState(new IntPtr(dte.MainWindow.HWnd), TBPF.INDETERMINATE);
        }
        #endregion

    }
}
