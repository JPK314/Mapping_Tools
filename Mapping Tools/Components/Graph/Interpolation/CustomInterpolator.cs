﻿using System.ComponentModel;

namespace Mapping_Tools.Components.Graph.Interpolation {
    [IgnoreInterpolator]
    [DisplayName("Custom")]
    public class CustomInterpolator : IGraphInterpolator {
        public delegate double InterpolationDelegate(double t, double p);

        public InterpolationDelegate InterpolationFunction { get; set; }
        public double P { get; set; } = 0;

        public CustomInterpolator() {
            InterpolationFunction = (t, p) => t;
        }

        public CustomInterpolator(InterpolationDelegate interpolationFunction) {
            InterpolationFunction = interpolationFunction;
        }

        public double GetInterpolation(double t) {
            return InterpolationFunction(t, P);
        }
    }
}