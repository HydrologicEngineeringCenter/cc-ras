using Geospatial.GDALAssist;
using Geospatial.IO;
using Geospatial.Rasters;
using Ras.Layers;
using Ras.Mapping;
using Utility.Extensions;
using Utility.Progress;

namespace cc_ras
{
    public class MapArgs
    {
        public string? ResultFilename { get; set; }
        public string? OutputFilename { get; set; }
        public string? TerrainFilename { get; set; }
        public double CellSize { get; set; }
        public int PfIdx { get; set; }
        public MapTypes MapType { get; set; }

        public bool Execute()
        {
            GDALSetup.InitializeMultiplatform();

            bool success;

            Result result = new(ResultFilename);

            success = GetTerrainAndProjection(out Projection srcPrj, out IResample<float> terr);
            if (!success)
                return false;


            var res = new ResultMapResampler(MapType, result.BaseOutputBlock, terr, result.Geometry.FlowAreaLayer.AllPropertyTables()); 
            IResample<float> band = res.AsSingleProfile(PfIdx);

            Geospatial.Vectors.Extent ext = result.Geometry.HydraulicExtent;
            RasterDefinition rdef = new(ext, CellSize);

            ProgressReporter rep = ProgressReporter.ConsoleWrite();
            return WriteMap(srcPrj, band, rdef, rep);
        }

        /// <summary>
        /// Writes the of a standard profile to file.
        /// </summary>
        private bool WriteMap(Projection outPrj, IResample<float> band, RasterDefinition rdef, ProgressReporter pr)
        {
            TiffExportEngine.ExportWithOverviews(OutputFilename, band, rdef, outPrj, pr);
            return true;
        }

        /// <summary>
        /// loads the terrain and projection from the file. tries to process as a ras terrain, tif, and finally with gdal reader. returns false
        /// if all fail. 
        /// </summary>
        private bool GetTerrainAndProjection(out Projection srcPrj, out IResample<float> terr)
        {
            if (TerrainFilename.EndsWith(".h5") || TerrainFilename.EndsWith(".hdf"))
            {
                Terrain t = new(TerrainFilename);
                srcPrj = t.TryGetProjection();
                terr = t;
                return true;
            }
            else if (TerrainFilename.EndsWith(".tif"))
            {
                // TODO: check int/short/etc
                var ds = new TiffDataSource<float>(TerrainFilename);
                terr = ds.AsRasterizer();
                srcPrj = GDALRaster.GetProjection(TerrainFilename);
                return true;
            }
            else
            {
                // TODO: Try gdal reader
                srcPrj = null;
                terr = null;
                Console.WriteLine("Unknown File/Extension: " + TerrainFilename.InDoubleQuotes());
                return false;
            }
        }
    }

    
}
