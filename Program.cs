using System.Xml;

namespace RedirectScan
{

	public record class Dep(string Name, string Version);

	internal class Program
	{
		static IEnumerable<Dep> LoadDeps(string filename)
		{
			var xml = new XmlDocument();
			xml.Load(filename);
			var nsm = new XmlNamespaceManager(xml.NameTable);
			nsm.AddNamespace("a", "urn:schemas-microsoft-com:asm.v1");
			var nodes = xml.SelectNodes("//a:dependentAssembly", nsm)!;
			foreach (XmlElement node in nodes)
			{
				var name = node.SelectSingleNode("a:assemblyIdentity/@name", nsm)!.Value!;
				var version = node.SelectSingleNode("a:bindingRedirect/@newVersion", nsm)!.Value!;
				yield return new Dep(name, version);
			}

		}

		static void Main(string[] args)
		{
			var filename = args.FirstOrDefault();
			if (string.IsNullOrEmpty(filename) || !File.Exists(filename))
			{
				Console.WriteLine("Usage: RedirectScan <filename>");
				return;
			}
			var deps = LoadDeps(filename).OrderBy(d => d.Name).ToList();
			foreach (var d in deps)
			{ 			
				Console.WriteLine($"{d.Name}\t{d.Version}");
			}
		}
	}
}