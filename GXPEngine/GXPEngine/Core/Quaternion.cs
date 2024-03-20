using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        static public Quaternion FromRotationAroundAxis(Vector3 axis, float rotationAngle)
        {
            return FromRotationAroundAxis(axis.x, axis.y, axis.z, rotationAngle);
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
            //ethically sourced from https://en.wikipedia.org/wiki/Conversion_between_quaternions_and_Euler_angles
            get
            {
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
        static public Quaternion operator *(Quaternion q1, Quaternion q2)
        {
            return Multiply(q1, q2);
        }

        static public Quaternion operator *(Quaternion q, float s)
        {
            return new Quaternion(q.r * s, q.i * s, q.j * s, q.k * s);
        }

        static public Quaternion operator +(Quaternion q1, Quaternion q2)
        {
            return new Quaternion(q1.r + q2.r, q1.i + q2.i, q1.j + q2.j, q1.k + q2.k);
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
            float magsq = r * r + i * i + j * j + k * k;
            if (magsq == 1) return;
            if (magsq == 0)
            {
                this = Identity;
                return;
            }
            float invMag = 1.0f / Mathf.Sqrt(magsq);
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
        /// Returns a normalized version of the quaternion in question.
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

        //as seen in https://en.wikipedia.org/wiki/Conversion_between_quaternions_and_Euler_angles#Rotation_matrices
        public Vector3 Left
        {
            get
            {
                return new Vector3(1f - 2f * (j * j + k * k), 2f * (i * j - r * k), 2f * (r * j + i * k));
            }
        }
        public Vector3 Up
        {
            get
            {
                return new Vector3(2f * (i * j + r * k), 1f - 2f * (i * i + k * k), 2f * (j * k - r * i));
            }
        }
        public Vector3 Forward
        {
            get
            {
                return new Vector3(2f * (i * k - r * j), 2f * (r * i + j * k), 1f - 2f * (i * i + j * j));
            }
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
        /// The direction's "forward". Usually this should be (1,0,0).
        /// </param>
        public static Quaternion LookTowards(Vector3 direction, Vector3 forward)
        {
            direction.Normalize();
            float angle = Mathf.Acos(direction * forward);
            Vector3 axis = (direction ^ forward).normalized();
            return FromRotationAroundAxis(axis, angle);
        }

        public static Quaternion LookTowards(Vector3 direction)
        {
            float signX = Mathf.Sign(direction.x);
            float signY = Mathf.Sign(direction.y);
            
            direction.Normalize();
            Vector3 left = Vector3.up ^ direction;
            left.Normalize();
            Vector3 up = left ^ direction;

            Quaternion res;

            float AngleXZ = Mathf.Acos(left * Vector3.left);
            float AngleY = Mathf.Acos(up * Vector3.up);

            res = FromRotationAroundAxis(Vector3.up, -AngleXZ * signX);
            res *= FromRotationAroundAxis(Vector3.left, -AngleY * signY);

            /*if (Input.GetKeyDown(Key.H))
            {
                Console.WriteLine(left);
                Console.WriteLine(up);
                Console.WriteLine(direction);
                Console.WriteLine("-------------");
                Console.WriteLine(AngleXZ);
                Console.WriteLine(AngleY);
                Console.WriteLine("-----------");
            }*/

            return res;
        }

        //------------------------------------------------------------------------------------------------------------------------
        //													SLerp()
        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Interpolates between two quaternions.
        /// </summary>
        /// <param name='q1'>
        /// The start quaternion. Associated with t=0.0.
        /// </param>
        /// <param name='q2'>
        /// The end quaternion. Associated with t=1.0.
        /// </param>
        /// <param name="t">
        /// The "time" in between the two quaternions. Decides how much to take from q1 and q2.
        /// </param>
        public static Quaternion SLerp(Quaternion q1, Quaternion q2, float t)
        {
            //based on https://www.euclideanspace.com/maths/algebra/realNormedAlgebra/quaternions/slerp/index.htm
            //i say based on. it wouldve been if i wrote this when i originally read the post
            //instead i put it off because i was busy doing """"more important"""" things
            //now i forgot what any of this does and i just copied the code sample word for word

            float cosHalfTheta = q1.Dot(q2);
            if (Mathf.Abs(cosHalfTheta) >= 1.0)
                return q1;
            
            float halfTheta = Mathf.Acos(cosHalfTheta);
            float sinHalfTheta = Mathf.Sqrt(1f - cosHalfTheta * cosHalfTheta);
            if (Mathf.Abs(sinHalfTheta) < .001f)
                return (q1+q2)*.5f;

            float ratioA = Mathf.Sin((1 - t) * halfTheta) / sinHalfTheta;
            float ratioB = Mathf.Sin(t*halfTheta) / sinHalfTheta;

            return q1*ratioA + q2*ratioB;
        }

        override public string ToString()
        {
            return "[Quat "+ r + ", " + i + ", " + j + ", " + k + "]";
        }
    }
}
