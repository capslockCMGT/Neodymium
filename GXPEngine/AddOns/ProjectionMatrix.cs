using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GXPEngine.Core;

namespace GXPEngine
{
    public class ProjectionMatrix
    {
        private readonly float[] _basis = new float[16] {
            1.0f, 0.0f, 0.0f, 0.0f,
            0.0f, 1.0f, 0.0f, 0.0f,
            0.0f, 0.0f, 1.0f, 0.0f,
            0.0f, 0.0f, 0.0f, 1.0f };
        private float[] _matrix = new float[16] {
            1.0f, 0.0f, 0.0f, 0.0f,
            0.0f, 1.0f, 0.0f, 0.0f,
            0.0f, 0.0f, 1.0f, 0.0f,
            0.0f, 0.0f, 0.0f, 1.0f };

        public float[] matrix
        {
            get { return (float[])_matrix.Clone(); }
        }

        /// <summary>
        /// Creates an orthographic projection matrix.
        /// Dimensions handles the size of the projection box,
        /// And near/far handle the near and far clipping planes.
        /// </summary>
        /// <param name="dimensions">Size of the projection box.</param>
        /// <param name="near">Near clipping plane. Usually 0.</param>
        /// <param name="far">Far clipping plane. Everything farther from the camera than this will not be rendered.</param>
        public ProjectionMatrix(Vector2 dimensions, float near, float far) {
            setOrthographic(dimensions, near, far);
        }

        public void setOrthographic(Vector2 dimensions, float near, float far)
        {
            //partially coming from: https://www.scratchapixel.com/lessons/3d-basic-rendering/perspective-and-orthographic-projection-matrix/orthographic-projection-matrix.html
            _basis.CopyTo(_matrix,0);
            _matrix[0] = 2f/dimensions.x;
            _matrix[5] = 2f/dimensions.y;
            _matrix[10] = -2f / (far - near);
            _matrix[14] = -(far+near)/(far-near);
        }

        /// <summary>
        /// Creates a perspective projection matrix.
        /// FOV sets the field of view in degrees,
        /// And near/far handle the near and far clipping planes.
        /// </summary>
        /// <param name="FOVX">The horizontal field of view, in degrees.</param>
        /// <param name="FOVY">The vertical field of view, in degrees.</param>
        /// <param name="near">Near clipping plane. Usually 0.</param>
        /// <param name="far">Far clipping plane. Everything farther from the camera than this will not be rendered.</param>
        public ProjectionMatrix(float FOVX, float FOVY, float near, float far)
        {
            setPerspective(FOVX, FOVY, near, far);
        }

        public void setPerspective(float FOVX, float FOVY, float near, float far)
        {
            //mostly coming from: https://www.scratchapixel.com/lessons/3d-basic-rendering/perspective-and-orthographic-projection-matrix/building-basic-perspective-projection-matrix.html
            _basis.CopyTo(_matrix, 0);
            _matrix[0] = 1 / Mathf.Tan((FOVX *.5f) * (Mathf.PI / 180));
            _matrix[5] = FOVX / (Mathf.Tan((FOVX *.5f) * (Mathf.PI / 180))*FOVY);
            _matrix[10] = -far / (far - near);
            _matrix[11] = -1;
            _matrix[14] = -far * near / (far - near);
            _matrix[15] = 0;
        }
    }
}
