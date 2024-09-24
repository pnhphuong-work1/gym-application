using System.Reflection;

namespace GymApplication.Services;

public static class AssemblyRef
{
    public static Assembly ServicesAssembly = typeof(AssemblyRef).Assembly;
}