/**
 * @Name code.cs
 * @Purpose Rulesengine output demo
 * @Date 10 August 2024, 11:53:02
 * @Author S.Deckers -  www.sdecomputing.nl
 * @Description 
 */

#region -- Using directives --
using Newtonsoft.Json;
using RulesEngine.Models;
#endregion

namespace OutputDemo;

public class Foo
{
    static void Main()
    {
        new Foo( ).multipleOccurrencesWithOutput( );
    }

	private void multipleOccurrencesWithOutput()
	{
		string currentDirectory = Directory.GetCurrentDirectory();
		string folder = Directory.GetParent(Directory.GetParent(Directory.GetParent(currentDirectory).FullName).FullName).FullName;

		var files = Directory.GetFiles(folder, "count_multiple_occurrences_output.json", SearchOption.AllDirectories);
		if (files == null || files.Length == 0)
		{
			throw new Exception("Rules not found.");
		}

		var fileData = File.ReadAllText(files[0]);
		var workflows = JsonConvert.DeserializeObject<List<Workflow>>(fileData);

		var stringList = new List<string> { "apple", "banana", "apple", "orange", "peach", "banana" };

		string workflowName = "FindFrequentFruitsWorkflow";

		List<RuleResultTree> resultList = new RulesEngine.RulesEngine(workflows.ToArray()).ExecuteAllRulesAsync(workflowName, stringList).Result;

		var tasks = resultList.ToList();

		int c = resultList.Where(x => x.IsSuccess == true).Count();

		System.Console.WriteLine($"{c} rules succeeded");

		foreach (var task in tasks)
		{
			System.Console.WriteLine($"Rule=[{task.Rule}] name=[{task.Rule.RuleName}], IsSuccess=[{task.IsSuccess}]");

			if (task.IsSuccess == false)
			{
				if (task.ExceptionMessage != task.Rule.ErrorMessage)
				{
					System.Console.WriteLine($"Invalid rule:[{task.ExceptionMessage}]");
					continue;
				}

				System.Console.WriteLine($"ErrorMessage=[{task.Rule.ErrorMessage}]");
				continue;
			}


			System.Console.WriteLine($"{task.Rule.SuccessEvent}");
			System.Console.WriteLine(task.Rule.Actions.ToString());
			string name = task.Rule.Actions.OnSuccess.Name;

			System.Console.WriteLine($"name=[{name}], value=[{task.ActionResult.Output}]");

			foreach (var item in task.Rule.Actions.OnSuccess.Context)
			{
				System.Console.WriteLine(item.ToString());
			}
		}
	}

}