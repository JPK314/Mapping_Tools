﻿using Mapping_Tools.Classes.MathUtil;
using Mapping_Tools.Classes.SnappingTools.DataStructure.RelevantObject.RelevantObjects;
using Mapping_Tools.Classes.SnappingTools.DataStructure.RelevantObjectGenerators.Allocation;
using Mapping_Tools.Classes.SnappingTools.DataStructure.RelevantObjectGenerators.GeneratorSettingses;
using Mapping_Tools.Classes.SnappingTools.DataStructure.RelevantObjectGenerators.GeneratorTypes;

namespace Mapping_Tools.Classes.SnappingTools.DataStructure.RelevantObjectGenerators.Generators {
    public class SinglePointCircleGenerator : RelevantObjectsGenerator {
        public override string Name => "Circle from Single Point";
        public override string Tooltip => "Generates circles with a specified radius on every virtual point.";
        public override GeneratorType GeneratorType => GeneratorType.Intermediate;

        private SinglePointCircleGeneratorSettings MySettings => (SinglePointCircleGeneratorSettings) Settings;

        /// <summary>
        /// Initializes SinglePointCircleGenerator with a custom settings object
        /// </summary>
        public SinglePointCircleGenerator() : base(new SinglePointCircleGeneratorSettings()) {
            Settings.Generator = this;

            Settings.IsActive = false;
            Settings.IsDeep = false;
            MySettings.Radius = 100;
        }

        [RelevantObjectsGeneratorMethod]
        public RelevantCircle GetRelevantObjects(RelevantPoint point) {
            return new RelevantCircle(new Circle(point.Child, MySettings.Radius));
        }
    }
}
