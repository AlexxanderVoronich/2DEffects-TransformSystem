using System;

namespace Assets.EffectsScripts
{
    public delegate double InternalTweenAlgorithm(double _inpart);

    public class tweenAlgorithmFactory
    {
        public enum eTweensAlgorithms
        {
            Linear,
            Elastic,
            InvertedParabola,
            ParabolaWithParameters,
            HalfLinear
        }


        public static InternalTweenAlgorithm getAlgorithm(eTweensAlgorithms _alg_type)
        {
            switch(_alg_type)
            {
                case eTweensAlgorithms.Linear:
                    {
                        return null;
                    }
                case eTweensAlgorithms.Elastic:
                    {
                        return simpleElasticMethod;
                    }
                case eTweensAlgorithms.InvertedParabola:
                    {
                        return invertedParabola;
                    }
                case eTweensAlgorithms.ParabolaWithParameters:
                    {
                        return null;
                    }
                case eTweensAlgorithms.HalfLinear:
                    {
                        return halfLinear;
                    }
                default:
                    {
                        return null;
                    }
            }
        }

        public static double simpleElasticMethod(double inpart)
        {
            double p = 0.3;
            return Math.Pow(2, -10 * inpart) * Math.Sin((inpart - p / 4)*(2 * Math.PI) / p) + 1;
        }

        public static double invertedParabola(double inpart)
        {
            if (inpart < 0.0f)
            {
                return 0.0f;
            }
            else if (inpart <= 1.0f)
            {
                return (-4) * (inpart - 0.5f) * (inpart - 0.5f) + 1;
            }
            return 1.0f;
        }

        public static double halfLinear(double inpart)
        {
            if (inpart < 0.5f)
            {
                return inpart;
            }
            else
            {
                return (1.0f - inpart);
            }
        }

        public static double parabolaWithParameters(double inpart, float _a, float _b, float _c)
        {
            return _a * inpart * inpart + _b * inpart + _c;
        }
    }
}
