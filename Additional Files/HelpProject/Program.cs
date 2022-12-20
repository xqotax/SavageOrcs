// See https://aka.ms/new-console-template for more information
using HelpProject;
using Newtonsoft.Json;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

Console.WriteLine("Hello, World!");



var jsonFromTxt1 = File.ReadAllText("1.txt");
var jsonFromTxt2 = File.ReadAllText("2.txt");
var settings = new JsonSerializerSettings { Error = (se, ev) => { ev.ErrorContext.Handled = true; } };
var list1 = JsonConvert.DeserializeObject<List<Area>>(jsonFromTxt1, settings);
var list2 = JsonConvert.DeserializeObject<List<Area>>(jsonFromTxt2, settings);

var resString = "";
var insertString = "\nInsert into [dbo].[Areas] ([Id] ,[Lvl_1] ,[Lvl_2] ,[Lvl_3] ,[Lvl_4] ,[AreaTypeId] ,[Name] ,[Code] ,[Region] ,[Community]) VALUES ";

int i = 0;
resString += insertString;
var b = list2[0];

foreach (var area in list1)
{
    var typeId = 0;
    if (area.object_category == "Місто")
        typeId = 1;
    else if (area.object_category == "СМТ")
        typeId = 2;
    else if (area.object_category == "Селище")
        typeId = 3;
    else if (area.object_category == "Село")
        typeId = 4;
    var l1 = area.level_1 != null ? area.level_1.ToString() : "NULL";
    var l2 = area.level_2 != null ? area.level_2.ToString() : "NULL";
    var l3 = area.level_3 != null ? area.level_3.ToString() : "NULL";
    var l4 = area.level_4 != null ? area.level_4.ToString() : "NULL";
    var code = area.object_code != null ? area.object_code.ToString() : "NULL";

    resString += "('" + Guid.NewGuid().ToString().ToUpper()
         + "', " + l1
         + ", " + l2
         + ", " + l3
         + ", " + l4
         + ", " + typeId.ToString()
         + ", '" + area.object_name.Replace("'", "''")
         + "', " + code
         + ", '" + area.region.ToString().Replace("'", "''")
         + "', '" + area.community.ToString().Replace("'", "''") + "')";

    if ((i != list2.Count - 1) && i < 950)
    {
        resString += ",\n";
    }
    else if ((i != list2.Count - 1) && (i >= 950))
    {
        resString += insertString;
        Console.WriteLine();
        i = 0;
    }
    i++;
    Console.Write(i.ToString() + " ");
}
File.WriteAllText("R1.txt", resString);

resString = "";
i = 0;
resString += insertString;

foreach (var area in list2)
{
    var typeId = 0;
    if (area.object_category == "Місто")
        typeId = 1;
    else if (area.object_category == "СМТ")
        typeId = 2;
    else if (area.object_category == "Селище")
        typeId = 3;
    else if (area.object_category == "Село")
        typeId = 4;
    var l1 = area.level_1 != null ? area.level_1.ToString() : "NULL";
    var l2 = area.level_2 != null ? area.level_2.ToString() : "NULL";
    var l3 = area.level_3 != null ? area.level_3.ToString() : "NULL";
    var l4 = area.level_4 != null ? area.level_4.ToString() : "NULL";
    var code = area.object_code != null ? area.object_code.ToString() : "NULL";

    resString += "('" + Guid.NewGuid().ToString().ToUpper()
         + "', " + l1
         + ", " + l2
         + ", " + l3
         + ", " + l4
         + ", " + typeId.ToString()
         + ", '" + area.object_name.Replace("'", "''")
         + "', " + code
         + ", '" + area.region.ToString().Replace("'", "''")
         + "', '" + area.community.ToString().Replace("'", "''") + "')";

    if ((i != list2.Count - 1) && i < 950)
    {
        resString += ",\n";
    }
    else if ((i != list2.Count - 1) && (i >= 950))
    {
        resString += insertString;
        Console.WriteLine();
        i = 0;
    }
    i++;
    Console.Write(i.ToString() + " ");
}
File.WriteAllText("R2.txt", resString);

return 0;

