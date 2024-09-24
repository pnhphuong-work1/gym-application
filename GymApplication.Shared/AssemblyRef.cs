using System.Reflection;

namespace GymApplication.Shared;

public static class AssemblyRef
{
    public static readonly Assembly SharedAssembly = typeof(AssemblyRef).Assembly;
}