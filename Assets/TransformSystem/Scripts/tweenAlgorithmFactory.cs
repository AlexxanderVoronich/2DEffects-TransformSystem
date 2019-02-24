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
            InvertedParabola
        }


        public static InternalTweenAlgorithm getAlgorithm(eTweensAlgorithms _alg_type)
        {
            switch(_alg_type)
            {
                case eTweensAlgorithms.Linear:
                    {
                        return null;
                        break;
                    }
                case eTweensAlgorithms.Elastic:
                    {
                        return simpleElasticMethod;
                        break;
                    }
                case eTweensAlgorithms.InvertedParabola:
                    {
                        return invertedParabola;
                        break;
                    }
                default:
                    {
                        return null;
                        break;
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
    }
}
