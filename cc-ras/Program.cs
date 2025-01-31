using Ras.Mapping;
using cc_ras;
using Usace.CC.Plugin;

class Test
{
    public static async Task Main()
    {
        string terrainFileName = null;
        string resultFileName = null;
        string outputFileName = "./output.tif";

        PluginManager pluginManager = await PluginManager.CreateAsync();
        Payload payload = pluginManager.Payload;

        // write the result file to local
        DataSource resultDS = pluginManager.getInputDataSource("Result File");
        string resultName = Path.GetFileName(resultDS.Paths[0]);
        resultFileName = resultName;
        byte[] resultBytes = await pluginManager.getFile(resultDS, 0);
        File.WriteAllBytes(resultName, resultBytes);

        // iterate through all of the terrain file paths and write to local
        DataSource terrainDS = pluginManager.getInputDataSource("Terrain File");
        for (int i = 0; i < terrainDS.Paths.Length; i++)
        {
            string path = terrainDS.Paths[i];
            string terrainName = Path.GetFileName(path);

            // only set the terrain file arg name for the .h5 file, not the HDFs
            if (Path.GetExtension(path) == ".h5")
                terrainFileName = terrainName;

            byte[] terrainBytes = await pluginManager.getFile(pluginManager.getInputDataSource("Terrain File"), i);
            File.WriteAllBytes(terrainName, terrainBytes);
        }
        if (terrainFileName == null)
            throw new Exception("No terrain file found");

        foreach (Usace.CC.Plugin.Action action in payload.Actions)
        {
            Console.WriteLine(action.Name);
            switch (action.Type)
            {
                case "write-map":
                    // generate the RAS map with the result file, terrain files, and parameters from action
                    MapArgs mapArgs = new MapArgs();
                    mapArgs.ResultFilename = resultFileName;
                    mapArgs.TerrainFilename = terrainFileName;
                    mapArgs.OutputFilename = outputFileName;
                    mapArgs.CellSize = int.Parse(action.Parameters["CellSize"]);
                    mapArgs.PfIdx = int.Parse(action.Parameters["PfIndx"]);
                    mapArgs.MapType = (MapTypes)Enum.Parse(typeof(MapTypes), action.Parameters["MapType"]);
                    mapArgs.Execute();

                    // write map output file to S3 store
                    DataSource outputDS = pluginManager.getOutputDataSource("Output File");
                    byte[] mapBytes = File.ReadAllBytes(Path.GetFileName(outputFileName));
                    MemoryStream ms = new MemoryStream(mapBytes);
                    bool success = await pluginManager.FileWriter(ms, outputDS, 0);
                    break;

                default:
                    Console.WriteLine("Invalid type \'" + action.Type + "\' for action \'" + action.Name + "\'");
                    break;
            }
        } 
    }
}