using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.TemplateEngine.Abstractions;
using Microsoft.TemplateEngine.Utils;
using Microsoft.TemplateEngine.Edge.Template;
using Microsoft.TemplateEngine.Edge.Settings;
using Microsoft.TemplateEngine.Orchestrator.RunnableProjects;
using Microsoft.TemplateEngine.Orchestrator.RunnableProjects.Config;
using Microsoft.TemplateEngine.Orchestrator.RunnableProjects.Macros;

namespace templateEnginePlayground
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			var builtIns = new Dictionary<Guid, Func<Type>>
			   {
				{ new Guid("0C434DF7-E2CB-4DEE-B216-D7C58C8EB4B3"), () => typeof(RunnableProjectGenerator) },
				{ new Guid("3147965A-08E5-4523-B869-02C8E9A8AAA1"), () => typeof(BalancedNestingConfig) },
				{ new Guid("3E8BCBF0-D631-45BA-A12D-FBF1DE03AA38"), () => typeof(ConditionalConfig) },
				{ new Guid("A1E27A4B-9608-47F1-B3B8-F70DF62DC521"), () => typeof(FlagsConfig) },
				{ new Guid("3FAE1942-7257-4247-B44D-2DDE07CB4A4A"), () => typeof(IncludeConfig) },
				{ new Guid("3D33B3BF-F40E-43EB-A14D-F40516F880CD"), () => typeof(RegionConfig) },
				{ new Guid("62DB7F1F-A10E-46F0-953F-A28A03A81CD1"), () => typeof(ReplacementConfig) },
				{ new Guid("370996FE-2943-4AED-B2F6-EC03F0B75B4A"), () => typeof(ConstantMacro) },
				{ new Guid("BB625F71-6404-4550-98AF-B2E546F46C5F"), () => typeof(EvaluateMacro) },
				{ new Guid("10919008-4E13-4FA8-825C-3B4DA855578E"), () => typeof(GuidMacro) },
				{ new Guid("F2B423D7-3C23-4489-816A-41D8D2A98596"), () => typeof(NowMacro) },
				{ new Guid("011E8DC1-8544-4360-9B40-65FD916049B7"), () => typeof(RandomMacro) },
				{ new Guid("8A4D4937-E23F-426D-8398-3BDBD1873ADB"), () => typeof(RegexMacro) },
				{ new Guid("B57D64E0-9B4F-4ABE-9366-711170FD5294"), () => typeof(SwitchMacro) },
				{ new Guid("10919118-4E13-4FA9-825C-3B4DA855578E"), () => typeof(CaseChangeMacro) }
			}.ToList();
			var preferences = new Dictionary<string, string>
			{
				{ "dotnet-cli-version", "0" }
			};
			ITemplateEngineHost host = new DefaultTemplateEngineHost("templateEnginePlayground", "v3", "en-US", preferences, builtIns);
			var environmentSettings = new EngineEnvironmentSettings(host, (env) => new SettingsLoader(env));
			var templateCreator = new TemplateCreator(environmentSettings);

			var p = environmentSettings.SettingsLoader.Components.OfType<IGenerator>();


			var _templateCache = new TemplateCache(environmentSettings);
			_templateCache.Scan("template_feed");
			_templateCache.WriteTemplateCaches();

			var templateInfo = templateCreator.List(false, (a, b) => new MatchInfo() {
				Kind = MatchKind.Partial,
				Location = MatchLocation.ShortName
			}).ToList().First().Info;
			if (Directory.Exists("output"))
				Directory.Delete("output", true);
			var result = templateCreator.InstantiateAsync(templateInfo, "hello", "world", "output", new Dictionary<string, string>(), false, true).Result;
			if (result.ResultInfo.PrimaryOutputs.Count != 1)
				throw new Exception($"Expected 1 PrimaryOutput, got {result.ResultInfo.PrimaryOutputs.Count}");
		}
	}
}
