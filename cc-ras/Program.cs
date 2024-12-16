using Ras.Mapping;
using cc_ras;
using Usace.CC.Plugin;

class Test
{
    private static void SetEnv(string name, string value)
    {
        Environment.SetEnvironmentVariable(name, value, EnvironmentVariableTarget.Process);
    }

    public static async Task Main()
    {
        SetEnv("CC" + "_" + EnvironmentVariables.AWS_ACCESS_KEY_ID, "USERNAME");
        SetEnv("CC" + "_" + EnvironmentVariables.AWS_SECRET_ACCESS_KEY, "PASSWORD");
        SetEnv("CC" + "_" + "S3_MOCK", "true");
        SetEnv("CC" + "_" + "S3_ENDPOINT", "http://127.0.0.1:9000");

        await CCSimulator.Setup("{}", manifestID: "1", eventNumber: "987",
                        eventID: "57", root: "data", pluginDefinition: "dss-to-csv",
                        profile: "CC", ccBucketName: "cc-bucket");

        PluginManager pm = await PluginManager.CreateAsync();
        pm.LogMessage("hello from RAS Plugin");
        var eventNumber = pm.EventNumber();
        pm.LogMessage("event number is: " + eventNumber); 

        //MapArgs mapArgs = new MapArgs();
        //mapArgs.ResultFilename = "C:\\Users\\HEC\\Documents\\RAS Projects\\Berryessa 2025 Dambreak\\Results\\Dam Break.h5";
        //mapArgs.TerrainFilename = "C:\\Users\\HEC\\Documents\\RAS Projects\\Berryessa 2025 Dambreak\\Terrains\\Terrain.h5";
        //mapArgs.OutputFilename = "C:\\Users\\HEC\\source\\repos\\cc-ras\\output.tif";
        //mapArgs.CellSize = 10;
        //mapArgs.PfIdx = 5;
        //mapArgs.MapType = MapTypes.Depth;

        //mapArgs.Execute();

    }


    
}