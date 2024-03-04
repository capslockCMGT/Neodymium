using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


//NEEEEEEEEEEEEEERD
//SHIIIIIIIIIIIT
namespace GXPEngine.Core
{
    public struct Quaternion
    {
        public float r;
        public float i;
        public float j;
        public float k;

        public Quaternion(float r, float i, float j, float k) {
            this.r = r;
            this.i = i;
            this.j = j;
            this.k = k;
        }

        static readonly Quaternion identityQuaternion = new Quaternion(1, 0, 0, 0);
        static public Quaternion Identity
        {
            get { return identityQuaternion; }
        }

        //------------------------------------------------------------------------------------------------------------------------
        //													FromRotationAroundAxis()
        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Returns a new quaternion defined as a rotation around an arbitrary axis.
        /// </summary>
        /// <returns>
        /// The resulting rotation.
        /// </returns>
        /// <param name='x'>
        /// The x component of the rotation axis.
        /// </param>
        /// <param name='y'>
        /// The y component of the rotation axis.
        /// </param>
        /// <param name='z'>
        /// The z component of the rotation axis.
        /// </param>
        /// <param name='rotationAngle'>
        /// The rotation around the axis. An angle of 0 returns the identity quaternion.
        /// </param>
        static public Quaternion FromRotationAroundAxis(float x, float y, float z, float rotationAngle)
        {
            float a = rotationAngle * .5f;
            float sn = Mathf.Sin(a);

            return new Quaternion(Mathf.Cos(a), x * sn, y * sn, z * sn);
        }

        static public Quaternion FromEulers(Vector3 angles)
        {
            Quaternion res = new Quaternion();
            res.Eulers = angles;
            return res; 
        }

        //------------------------------------------------------------------------------------------------------------------------
        //													Eulers
        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Get or set a Quaternion using Euler rotations (AKA angles). First it rotates by Z, then X, then Y (like in Unity). It's in radians. Sorry.
        /// </summary>
        public Vector3 Eulers
        {
            get {
                //ethically sourced from https://en.wikipedia.org/wiki/Conversion_between_quaternions_and_Euler_angles
                //a little optimisation
                float slop = 2 * (r*j + i*k);
                float jj = j * j;
                return new Vector3(
                    Mathf.Atan2(2*(r*i + j*k), 1-2*(r*r+jj)),
                    -.5f*Mathf.PI + 2*Mathf.Atan2(Mathf.Sqrt(1+slop), Mathf.Sqrt(1-slop)),
                    Mathf.Atan2(2*(r*k + i*j), 1-2*(jj-k*k))
                    ); 
            }
            set {
                //make the quaternion the rotation of z
                r = Mathf.Cos(value.z);
                i = 0;
                j = 0;
                k = Mathf.Sin(value.z);
                //rotate it by x
                Quaternion temp = new Quaternion(Mathf.Cos(value.x), Mathf.Sin(value.x), 0, 0);
                Multiply(temp);
                //rotate it by y
                temp.r = Mathf.Cos(value.y);
                temp.i = 0;
                temp.j = Mathf.Sin(value.y);
                Multiply(temp);
            }
        }

        //------------------------------------------------------------------------------------------------------------------------
        //													Multiply()
        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Multiplies two quaternions, effectively rotating one.
        /// </summary>
        /// <returns>
        /// The product of the quaternions.
        /// </returns>
        /// <param name='q'>
        /// The quaternion to rotate by.
        /// </param>
        public void Multiply (Quaternion q)
        {
            float rtemp = r;
            float itemp = i;
            float jtemp = j;
            r = r * q.r - i * q.i - j * q.j - k * q.k;
            i = rtemp * q.i + i * q.r + k * q.j - j * q.k;
            j = rtemp * q.j + j * q.r + itemp * q.k - k * q.i;
            k = rtemp * q.k + k * q.r + jtemp * q.i - itemp * q.j;
        }
        static public Quaternion Multiply(Quaternion q1, Quaternion q2)
        {
            return new Quaternion(
                q1.r * q2.r - q1.i * q2.i - q1.j * q2.j - q1.k * q2.k,
                q1.r * q2.i + q1.i * q2.r + q1.k * q2.j - q1.j * q2.k,
                q1.r * q2.j + q1.j * q2.r + q1.i * q2.k - q1.k * q2.i,
                q1.r * q2.k + q1.k * q2.r + q1.j * q2.i - q1.i * q2.j
                );
        }
        static public Quaternion operator * (Quaternion q1, Quaternion q2)
        {
            return Multiply(q1, q2);
        }

        //------------------------------------------------------------------------------------------------------------------------
        //													Dot()
        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Returns the dot product of two quaternions.
        /// </summary>
        /// <returns>
        /// The dot product. The result is between 0 and 1, 1.0 for equal and 0.0 for opposite.
        /// </returns>
        /// <param name='q'>
        /// The quaternion to compute the dot product with.
        /// </param>
        public float Dot( Quaternion q )
        {
            return r*q.r + i*q.i + j*q.j + k*q.k; 
        }

        //------------------------------------------------------------------------------------------------------------------------
        //													Normalize()
        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Normalizes the quaternion in question.
        /// </summary>
        public void Normalize()
        {
            float invMag = 1.0f / Mathf.Sqrt(r*r + i*i + j*j + k*k);
            r *= invMag;
            i *= invMag;
            j *= invMag;
            k *= invMag;
            return;
        }

        //------------------------------------------------------------------------------------------------------------------------
        //													Normalized()
        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Normalizes the quaternion in question.
        /// </summary>
        public Quaternion Normalized()
        {
            Quaternion res = new Quaternion( r, i, j, k );
            res.Normalize();
            return res;
        }

        //------------------------------------------------------------------------------------------------------------------------
        //													Inverse()
        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Returns the inverse rotation of the quaternion in question.
        /// </summary>
        public Quaternion Inverse()
        {
            return new Quaternion( r, -i, -j, -k);
        }

        //------------------------------------------------------------------------------------------------------------------------
        //													LookTowards()
        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Makes a quaternion a rotation towards a direction.
        /// </summary>
        /// <param name='direction'>
        /// The direction to look towards.
        /// </param>
        /// <param name='up'>
        /// The direction's "up". Usually this should be (0,1,0).
        /// </param>
        public Quaternion LookTowards(Vector3 direction, Vector3 up)
        {
            Vector3 camside = Vector3.normalize(direction).cross( up );
            Vector3 camup = camside.cross( up );
            float q = Mathf.Sqrt(1 + camside.x + camup.y + direction.z) / 2;
            float q4 = 4 * q;
            //matrix conversion from https://www.euclideanspace.com/maths/geometry/rotations/conversions/matrixToQuaternion/
            return new Quaternion(
                q,
                (direction.y - camup.z)/q4,
                (camside.z - direction.x)/q4,
                (camup.x - camside.y)/q4
                );
        }

        override public string ToString()
        {
            return "[Quat "+ r + ", " + i + ", " + j + ", " + k + "]";
        }
    }
}
