using System;
using System.Numerics;

namespace CG_Project_V
{
    public static class Camera
    {
        public static double width = 200;
        public static double height = 200;
        public static double x = 0;
        public static double y = 0;
        public static double distance = 6;
        public static double targetX = 0;
        public static double targetY = 0;
        public static double targetZ = 0;

        public static Matrix4x4 Matrix()
        {
            (double, double, double) cPos, cTarget, cUp, cX, cY, cZ, tmp;
            double length;
            cPos = (x, y, distance);
            cTarget = (targetX, targetY, targetZ);
            cUp = (0, 1, 0);
            //cZ
            tmp = (cPos.Item1 - cTarget.Item1, cPos.Item2 - cTarget.Item2, cPos.Item3 - cTarget.Item3);
            length = Algorithms.LengthOfVector(tmp);
            cZ = (tmp.Item1 / length, tmp.Item2 / length, tmp.Item3 / length);
            //cX
            tmp = Algorithms.CrossProdOfVectors(cUp, cZ);
            length = Algorithms.LengthOfVector(tmp);
            cX = (tmp.Item1 / length, tmp.Item2 / length, tmp.Item3 / length);
            //cY
            tmp = Algorithms.CrossProdOfVectors(cZ, cX);
            length = Algorithms.LengthOfVector(tmp);
            cY = (tmp.Item1 / length, tmp.Item2 / length, tmp.Item3 / length);
            //result
            Matrix4x4 result = new Matrix4x4((float)cX.Item1, (float)cX.Item2, (float)cX.Item3, (float)Algorithms.MultiplicationOfVectors(cX, cPos),
                                            (float)cY.Item1, (float)cY.Item2, (float)cY.Item3, (float)Algorithms.MultiplicationOfVectors(cY, cPos),
                                            (float)cZ.Item1, (float)cZ.Item2, (float)cZ.Item3, (float)Algorithms.MultiplicationOfVectors(cZ, cPos),
                                            0, 0, 0, 1);
            return result;
        }
    }

    public class Transformation
    {
        public double alpha = 0;
        public double beta = 0;
        public double hori = 0;
        public double vert = 0;
    }
}
