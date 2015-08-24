﻿using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: AssemblyCompany("Microsoft")]
[assembly: AssemblyCopyright("Copyright © Microsoft. All Rights Reserved.")]

[assembly: ComVisible(false)]

[assembly: InternalsVisibleTo("Microsoft.ApplicationInsights.Core.Net35.Tests" + AssemblyInfo.PublicKey)]
[assembly: InternalsVisibleTo("Microsoft.ApplicationInsights.Core.Net40.Tests" + AssemblyInfo.PublicKey)]
[assembly: InternalsVisibleTo("Microsoft.ApplicationInsights.Core.Net46.Tests" + AssemblyInfo.PublicKey)]
[assembly: InternalsVisibleTo("Microsoft.ApplicationInsights.Core.Win81.Tests" + AssemblyInfo.PublicKey)]
[assembly: InternalsVisibleTo("Microsoft.ApplicationInsights.Core.Wp80.Tests" + AssemblyInfo.PublicKey)]
[assembly: InternalsVisibleTo("Microsoft.ApplicationInsights.Core.Wpa81.Tests" + AssemblyInfo.PublicKey)]
[assembly: InternalsVisibleTo("Microsoft.ApplicationInsights.Core.Uwp.Tests" + AssemblyInfo.PublicKey)]

[assembly: InternalsVisibleTo("Microsoft.ApplicationInsights.TestFramework" + AssemblyInfo.PublicKey)]

[assembly: InternalsVisibleTo("Microsoft.ApplicationInsights.Extensibility.Windows" + AssemblyInfo.PublicKey)]
[assembly: InternalsVisibleTo("Microsoft.ApplicationInsights.Windows.NuGet.Tests" + AssemblyInfo.PublicKey)]
[assembly: InternalsVisibleTo("Microsoft.ApplicationInsights.Windows.Wp80.Tests" + AssemblyInfo.PublicKey)]
[assembly: InternalsVisibleTo("Microsoft.ApplicationInsights.Windows.Wpa81.Tests" + AssemblyInfo.PublicKey)]
[assembly: InternalsVisibleTo("Microsoft.ApplicationInsights.Windows.Win81.Tests" + AssemblyInfo.PublicKey)]

[assembly: InternalsVisibleTo("Microsoft.ApplicationInsights.WindowsChannel" + AssemblyInfo.PublicKey)]

[assembly: InternalsVisibleTo("Microsoft.ApplicationInsights.PersistenceChannel" + AssemblyInfo.PublicKey)]

[assembly: InternalsVisibleTo("Microsoft.ApplicationInsights.PersistenceChannel.Net40.Tests" + AssemblyInfo.PublicKey)]
[assembly: InternalsVisibleTo("Microsoft.ApplicationInsights.PersistenceChannel.Net45.Tests" + AssemblyInfo.PublicKey)]
[assembly: InternalsVisibleTo("Microsoft.ApplicationInsights.PersistenceChannel.Wrt81.Tests" + AssemblyInfo.PublicKey)]
[assembly: InternalsVisibleTo("Microsoft.ApplicationInsights.PersistenceChannel.Wp80.Tests" + AssemblyInfo.PublicKey)]

// This is for RDD
#if PUBLIC_RELEASE
[assembly: InternalsVisibleTo("Microsoft.EnterpriseManagement.OperationsManager.Apm.RuntimeDiscovery, PublicKey=0024000004800000940000000602000000240000525341310004000001000100b5fc90e7027f67871e773a8fde8938c81dd402ba65b9201d60593e96c492651e889cc13f1415ebb53fac1131ae0bd333c5ee6021672d9718ea31a8aebd0da0072f25d87dba6fc90ffd598ed4da35e44c398c454307e8e33b8426143daec9f596836f97c8f74750e5975c64e2189f45def46b2a2b1247adc3652bf5c308055da9")]
#endif

internal static class AssemblyInfo
{
#if PUBLIC_RELEASE
    // Public key; assemblies are delay signed.
    public const string PublicKey = ", PublicKey=0024000004800000940000000602000000240000525341310004000001000100b5fc90e7027f67871e773a8fde8938c81dd402ba65b9201d60593e96c492651e889cc13f1415ebb53fac1131ae0bd333c5ee6021672d9718ea31a8aebd0da0072f25d87dba6fc90ffd598ed4da35e44c398c454307e8e33b8426143daec9f596836f97c8f74750e5975c64e2189f45def46b2a2b1247adc3652bf5c308055da9";
    public const string MoqPublicKey = ", PublicKey=0024000004800000940000000602000000240000525341310004000001000100c547cac37abd99c8db225ef2f6c8a3602f3b3606cc9891605d02baa56104f4cfc0734aa39b93bf7852f7d9266654753cc297e7d2edfe0bac1cdcf9f717241550e0a7b191195b7667bb4f64bcb8e2121380fd1d9d46ad2d92d2d15605093924cceaf74c4861eff62abf69b9291ed0a340e113be11e6a7d3113e92484cf7045cc7";
#elif PRIVATE_RELEASE
    // Internal key; assemblies are fully signed.
    public const string PublicKey = ", PublicKey=0024000004800000940000000602000000240000525341310004000001000100319b35b21a993df850846602dae9e86d8fbb0528a0ad488ecd6414db798996534381825f94f90d8b16b72a51c4e7e07cf66ff3293c1046c045fafc354cfcc15fc177c748111e4a8c5a34d3940e7f3789dd58a928add6160d6f9cc219680253dcea88a034e7d472de51d4989c7783e19343839273e0e63a43b99ab338149dd59f";
    public const string MoqPublicKey = ", PublicKey=0024000004800000940000000602000000240000525341310004000001000100c547cac37abd99c8db225ef2f6c8a3602f3b3606cc9891605d02baa56104f4cfc0734aa39b93bf7852f7d9266654753cc297e7d2edfe0bac1cdcf9f717241550e0a7b191195b7667bb4f64bcb8e2121380fd1d9d46ad2d92d2d15605093924cceaf74c4861eff62abf69b9291ed0a340e113be11e6a7d3113e92484cf7045cc7";
#else
    // Internal key; assemblies are fully signed.
    public const string PublicKey = "";
    public const string MoqPublicKey = "";
#endif
}