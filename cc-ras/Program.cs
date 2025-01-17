﻿using Ras.Mapping;
using cc_ras;
using Usace.CC.Plugin;

class Test
{
    public static async Task Main()
    {
        string terrainFileName = null;
        string resultFileName = null;
        string outputFileName = "./output.tif";

        InitEnv();

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

            if (Path.GetExtension(path) == ".h5")
                terrainFileName = terrainName;

            byte[] terrainBytes = await pluginManager.getFile(pluginManager.getInputDataSource("Terrain File"), i);
            File.WriteAllBytes(terrainName, terrainBytes);
        }
        if (terrainFileName == null)
            throw new Exception("No terrain file found");

        foreach (Usace.CC.Plugin.Action action in payload.Actions)
        {
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
        } 
    }

    private static void SetEnv(string name, string value)
    {
        Environment.SetEnvironmentVariable(name, value, EnvironmentVariableTarget.Process);
    }

    private static void InitEnv()
    {
        SetEnv(EnvironmentVariables.CC_PROFILE, "CC");
        SetEnv(EnvironmentVariables.CC_PROFILE + "_" + EnvironmentVariables.AWS_DEFAULT_REGION, "us-west-2");
        SetEnv(EnvironmentVariables.CC_PROFILE + "_" + EnvironmentVariables.AWS_ACCESS_KEY_ID, "USERNAME");
        SetEnv(EnvironmentVariables.CC_PROFILE + "_" + EnvironmentVariables.AWS_SECRET_ACCESS_KEY, "PASSWORD");
        SetEnv(EnvironmentVariables.CC_PROFILE + "_" + EnvironmentVariables.AWS_S3_BUCKET, "cc-store");
        SetEnv("CC_S3_ENDPOINT", "http://127.0.0.1:9000");
        SetEnv("CC_S3_MOCK", "true");

        SetEnv(EnvironmentVariables.CC_MANIFEST_ID, "1");
        SetEnv(EnvironmentVariables.CC_EVENT_NUMBER, "987");
        SetEnv(EnvironmentVariables.CC_EVENT_ID, "57");
        SetEnv(EnvironmentVariables.CC_ROOT, "cc_store");
        SetEnv(EnvironmentVariables.CC_PLUGIN_DEFINITION, "ras");

        SetEnv("FFRD_AWS_DEFAULT_REGION", "us-west-2");
        SetEnv("FFRD_AWS_ACCESS_KEY_ID", "USERNAME");
        SetEnv("FFRD_AWS_SECRET_ACCESS_KEY", "PASSWORD");
        SetEnv("FFRD_AWS_S3_BUCKET", "model-library");
        SetEnv("FFRD_S3_ENDPOINT", "http://127.0.0.1:9000");
        SetEnv("FFRD_S3_MOCK", "true");
    }
}