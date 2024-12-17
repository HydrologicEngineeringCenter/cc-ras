using Ras.Mapping;
using cc_ras;
using Usace.CC.Plugin;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Utility.Extensions.Attributes;

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
        SetEnv("CC" + "_" + EnvironmentVariables.AWS_DEFAULT_REGION, "us-west-2");
        SetEnv("CC_S3_MOCK", "true");
        SetEnv("CC_S3_ENDPOINT", "http://127.0.0.1:9000");
        SetEnv(EnvironmentVariables.CC_MANIFEST_ID, "1");
        SetEnv(EnvironmentVariables.CC_EVENT_NUMBER, "987");
        SetEnv(EnvironmentVariables.CC_EVENT_ID, "57");
        SetEnv(EnvironmentVariables.CC_ROOT, "data");
        SetEnv(EnvironmentVariables.CC_PLUGIN_DEFINITION, "ras");
        SetEnv(EnvironmentVariables.CC_PROFILE, "CC");
        SetEnv(EnvironmentVariables.CC_PROFILE + "_" + EnvironmentVariables.AWS_S3_BUCKET, "cc-bucket");

        // this payload JSON lives in the cc-bucket in my MINIO testing environment server
        /*
        string payloadString = File.ReadAllText("payload-ras.json");
        await CCSimulator.Setup(payloadString, manifestID: "1", eventNumber: "987",
                        eventID: "57", root: "data", pluginDefinition: "dss-to-csv",
                        profile: "CC", ccBucketName: "cc-bucket"); 
        */


        PluginManager pm = await PluginManager.CreateAsync();
        pm.LogMessage("hello from RAS Plugin");

        Payload p = pm.Payload;
        
        MapArgs mapArgs = new MapArgs();
        mapArgs.ResultFilename = pm.getInputDataSource("Result File").Paths[0];
        mapArgs.TerrainFilename = pm.getInputDataSource("Terrain File").Paths[0];
        mapArgs.OutputFilename = pm.getOutputDataSource("Output File").Paths[0];
        mapArgs.CellSize = int.Parse(p.Attributes["CellSize"]);
        mapArgs.PfIdx = int.Parse(p.Attributes["PfIndx"]);
        mapArgs.MapType = (MapTypes)Enum.Parse(typeof(MapTypes), p.Attributes["MapType"]);

        mapArgs.Execute();

    }
}