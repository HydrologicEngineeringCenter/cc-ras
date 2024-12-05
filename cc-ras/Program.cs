using Geospatial.IO;
using Geospatial.Rasters;
using Ras.Layers;
using Ras.Mapping;
using Usace.CC.Plugin;
using Utility.Progress;
using Geospatial.GDALAssist;
using Utility.Extensions;
using System.Data.SQLite;
using cc_ras;

class Test
{
    private static void SetEnv(string name, string value)
    {
        Environment.SetEnvironmentVariable(name, value, EnvironmentVariableTarget.Process);
    }

    public static void Main()
    {
        //SetEnv("CC" + "_" + EnvironmentVariables.AWS_S3_BUCKET, "test-bucket-1");
        //SetEnv("CC" + "_" + EnvironmentVariables.AWS_ACCESS_KEY_ID, "USERNAME");
        //SetEnv("CC" + "_" + EnvironmentVariables.AWS_SECRET_ACCESS_KEY, "PASSWORD");
        //SetEnv("CC" + "_" + "S3_MOCK", "true");
        //SetEnv("CC" + "_" + "S3_ENDPOINT", "http://127.0.0.1:9000");

        //var bucket = new AwsBucket("CC");
        //await bucket.CreateBucketIfNotExists();

        //string key = "MINIO-test.txt";
        //bool exists = await bucket.ObjectExists(key);
        //if (!exists)
        //{
        //    throw new Exception("object not found");
        //}
        //string input = await bucket.ReadObjectAsText(key);
        //Console.WriteLine(input);

        //string compute = input + " and this was appended";
        //string computeKey = "MINIO-test-compute.txt";
        //await bucket.CreateObject(computeKey, compute);

        //string result = await bucket.ReadObjectAsText(computeKey);
        //Console.WriteLine(result);

        //PluginManager pm = await PluginManager.CreateAsync();
        //Payload payload = pm.Payload;

        MapArgs mapArgs = new MapArgs();
        mapArgs.ResultFilename = "C:\\Users\\HEC\\Documents\\RAS Projects\\Berryessa 2025 Dambreak\\Results\\Dam Break.h5";
        mapArgs.TerrainFilename = "C:\\Users\\HEC\\Documents\\RAS Projects\\Berryessa 2025 Dambreak\\Terrains\\Terrain.h5";
        mapArgs.OutputFilename = "C:\\Users\\HEC\\source\\repos\\cc-ras\\output.tif";
        mapArgs.CellSize = 10;
        mapArgs.PfIdx = 5;
        mapArgs.MapType = MapTypes.Depth;

        mapArgs.Execute();

    }


    
}