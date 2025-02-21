using Geospatial.GDALAssist;
using Geospatial.IO;
using Geospatial.Rasters;
using Geospatial.Vectors;
using Ras.Layers;
using Ras.Mapping;
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

        public void CreateMapFromResult()
        {
            GDALSetup.InitializeMultiplatform();

            Terrain terrain = new(TerrainFilename);
            ProgressReporter pr = ProgressReporter.ConsoleWrite(true);
            Projection srcPrj = terrain.TryGetProjection();

            Result result = new(ResultFilename);
            var res = new ResultMapResampler(MapType, result.BaseOutputBlock, terrain, result.Geometry.FlowAreaLayer.AllPropertyTables());
            IResample<float> profile = res.AsSingleProfile(PfIdx);
            Extent ext = result.Geometry.HydraulicExtent;
            RasterDefinition rdef = new(ext, CellSize);

            TiffExportEngine.ExportWithOverviews(OutputFilename, profile, rdef, srcPrj, pr);
        }
    }
}