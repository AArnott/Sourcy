#pragma warning disable RS1035

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Sourcy;

public abstract class BaseSourcyGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var incrementalValuesProvider = context.SyntaxProvider
                .ForAttributeWithMetadataName("Sourcy.EnableSourcyAttribute", 
                    static (_, _) => true, 
                    static (syntaxContext, _) => (string)syntaxContext.Attributes.First().ConstructorArguments.First().Value!)
                .Collect();
        
        context.RegisterSourceOutput(incrementalValuesProvider, (productionContext, paths) => Initialize(productionContext, GetRootDirectory(paths[0])));
    }

    protected abstract void Initialize(SourceProductionContext context, Root root);

    protected static Root GetRootDirectory(string path)
    {
        var location = GetLocation(path);

        while (true)
        {
            if (location == location.Root)
            {
                break;
            }
            
            if (Directory.Exists(Path.Combine(location.FullName, ".git")))
            {
                return new Root(location);
            }
            
            if (File.Exists(Path.Combine(location.FullName, ".sourcyroot")))
            {
                return new Root(location);
            }
            
            var parent = location.Parent;

            if (parent is null || parent == location)
            {
                return new Root(location);
            }

            location = parent;
        }

        return new Root(location);
    }

    protected static DirectoryInfo GetLocation(string path)
    {
        return new FileInfo(path).Directory!;
    }

    protected static SourceText GetSourceText([StringSyntax("c#")] string code)
    {
        return SourceText.From(code, Encoding.UTF8);
    }
}