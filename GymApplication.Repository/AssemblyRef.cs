using System.Reflection;

namespace GymApplication.Repository;

public static class AssemblyRef
{
    public static Assembly RepositoryAssembly => typeof(AssemblyRef).Assembly;
}