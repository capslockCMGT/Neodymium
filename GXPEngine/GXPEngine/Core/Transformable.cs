using System;
using GXPEngine.Core;

namespace GXPEngine
{
	/// <summary>
	/// The Transformable class contains all positional data of GameObjects.
	/// </summary>
	public class Transformable
	{

		protected float[] _matrix = new float[16] { 
			1.0f, 0.0f, 0.0f, 0.0f,
			0.0f, 1.0f, 0.0f, 0.0f,
			0.0f, 0.0f, 1.0f, 0.0f,
			0.0f, 0.0f, 0.0f, 1.0f };

        // i^ i^ i^ 0
        // j^ j^ j^ 0
        // k^ k^ k^ 0
        // x  y  z  1
			
		protected Quaternion _rotation = Quaternion.Identity;
        protected bool _rotationMatrixIsUpToDate = true;
        protected int _rotationUpdates = 0;
        protected float _scaleX = 1.0f;
		protected float _scaleY = 1.0f;
        protected float _scaleZ = 1.0f;

        //------------------------------------------------------------------------------------------------------------------------
        //														Transform()
        //------------------------------------------------------------------------------------------------------------------------
        public Transformable () {
		}

        //------------------------------------------------------------------------------------------------------------------------
        //														GetMatrix()
        //------------------------------------------------------------------------------------------------------------------------		
        /// <summary>
        /// Returns the gameobject's 4x4 matrix.
        /// </summary>
        /// <value>
        /// The matrix.
        /// </value>
        public float[] matrix
        {
            get
            {
                UpdateRotationMatrix();
                float[] matrix = (float[])_matrix.Clone();
                matrix[0] *= _scaleX;
                matrix[1] *= _scaleX;
                matrix[2] *= _scaleX;
                matrix[4] *= _scaleY;
                matrix[5] *= _scaleY;
                matrix[6] *= _scaleY;
                matrix[8] *= _scaleZ;
                matrix[9] *= _scaleZ;
                matrix[10] *= _scaleZ;
                return matrix;
            }
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														position
        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <value>
        /// All of it.
        /// </value>
        public Vector3 position
        {
            get { return new Vector3(x, y, z); } 
            set { x = value.x; y = value.y; z = value.z;}
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														x
        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the x position.
        /// </summary>
        /// <value>
        /// The x.
        /// </value>
        public float x {
			get { return _matrix[12]; }
			set { _matrix[12] = value; }
		}

        //------------------------------------------------------------------------------------------------------------------------
        //														y
        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the y position.
        /// </summary>
        /// <value>
        /// The y.
        /// </value>
        public float y
        {
            get { return _matrix[13]; }
            set { _matrix[13] = value; }
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														z
        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the z position.
        /// </summary>
        /// <value>
        /// The z.
        /// </value>
        public float z
        {
            get { return _matrix[14]; }
            set { _matrix[14] = value; }
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														SetXY
        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Sets the X, Y and Z position.
        /// </summary>
        /// <param name='x'>
        /// The x coordinate.
        /// </param>
        /// <param name='y'>
        /// The y coordinate.
        /// </param>
        /// <param name='z'>
        /// The z coordinate.
        /// </param>
        public void SetXY(float x, float y, float z) {
			_matrix[12] = x;
			_matrix[13] = y;
			_matrix[14] = z;
		}

        //------------------------------------------------------------------------------------------------------------------------
        //													InverseTransformPoint()
        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Transforms the point from the game's global space to this object's local space.
        /// </summary>
        /// <returns>
        /// The point.
        /// </returns>
        /// <param name='x'>
        /// The x coordinate.
        /// </param>
        /// <param name='y'>
        /// The y coordinate.
        /// </param>
        /// <param name='z'>
        /// The z coordinate.
        /// </param>
        public virtual Vector3 InverseTransformPoint(float x, float y, float z)
        {
            Vector3 ret = new Vector3();
            x -= _matrix[12];
            y -= _matrix[13];
            z -= _matrix[14];
            UpdateRotationMatrix();
            if (_scaleX != 0) ret.x = ((x * _matrix[0] + y * _matrix[1] + z * _matrix[2]) / _scaleX); else ret.x = 0;
            if (_scaleY != 0) ret.y = ((x * _matrix[4] + y * _matrix[5] + z * _matrix[6]) / _scaleY); else ret.y = 0;
            if (_scaleZ != 0) ret.z = ((x * _matrix[8] + y * _matrix[9] + z * _matrix[10]) / _scaleZ); else ret.z = 0;
            return ret;
        }
        /// <summary>
        /// Transforms the point from the game's global space to this object's local space.
        /// </summary>
        /// <returns>
        /// The point.
        /// </returns>
        /// <param name='point'>
        /// The position.
        /// </param>
        public virtual Vector3 InverseTransformPoint(Vector3 point)
        {
            return InverseTransformPoint(point.x, point.y, point.z);
        }

        /// <summary>
		/// Transforms the direction vector (x,y,z) from the game's global space to this object's local space.
		/// This means that rotation and scaling is applied, but translation is not.
		/// </summary>
		public virtual Vector3 InverseTransformDirection(float x, float y, float z)
        {
            Vector3 ret = new Vector3();
            UpdateRotationMatrix();
            if (_scaleX != 0) ret.x = ((x * _matrix[0] + y * _matrix[1] + z * _matrix[2]) / _scaleX); else ret.x = 0;
            if (_scaleY != 0) ret.y = ((x * _matrix[4] + y * _matrix[5] + z * _matrix[6]) / _scaleY); else ret.y = 0;
            if (_scaleZ != 0) ret.z = ((x * _matrix[8] + y * _matrix[9] + z * _matrix[10]) / _scaleZ); else ret.z = 0;
            return ret;
        }
        /// <summary>
		/// Transforms the direction vector from the game's global space to this object's local space.
		/// This means that rotation and scaling is applied, but translation is not.
		/// </summary>
        public virtual Vector3 InverseTransformDirection(Vector3 dir)
        {
            return InverseTransformDirection(dir.x, dir.y, dir.z);
        }


        //------------------------------------------------------------------------------------------------------------------------
        //														TransformPoint()
        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Transforms the point from this object's local space to the game's global space.
        /// </summary>
        /// <returns>
        /// The point.
        /// </returns>
        /// <param name='x'>
        /// The x coordinate.
        /// </param>
        /// <param name='y'>
        /// The y coordinate.
        /// </param>
        /// <param name='z'>
        /// The z coordinate.
        /// </param> 
        public virtual Vector3 TransformPoint(float x, float y, float z)
        {
            Vector3 ret = new Vector3();
            UpdateRotationMatrix();
            ret.x = (_matrix[0] * x * _scaleX + _matrix[4] * y * _scaleY + _matrix[8] * z * _scaleZ + _matrix[12]);
            ret.y = (_matrix[1] * x * _scaleX + _matrix[5] * y * _scaleY + _matrix[9] * z * _scaleZ + _matrix[13]);
            ret.z = (_matrix[2] * x * _scaleX + _matrix[6] * y * _scaleY + _matrix[10] * z * _scaleZ + _matrix[14]);
            return ret;
        }
        /// <summary>
        /// Transforms the point from this object's local space to the game's global space.
        /// </summary>
        /// <returns>
        /// The point.
        /// </returns>
        /// <param name='point'>
        /// The coordinates.
        /// </param>
        public virtual Vector3 TransformPoint(Vector3 point)
        {
            return TransformPoint(point.x, point.y, point.z);
        }

        /// <summary>
		/// Transforms a direction vector (x,y,z) from this object's local space to the game's global space. 
		/// This means that rotation and scaling is applied, but translation is not.
		/// </summary>
		public virtual Vector3 TransformDirection(float x, float y, float z)
        {
            Vector3 ret = new Vector3();
            UpdateRotationMatrix();
            ret.x = (_matrix[0] * x * _scaleX + _matrix[4] * y * _scaleY + _matrix[8] * z * _scaleZ);
            ret.y = (_matrix[1] * x * _scaleX + _matrix[5] * y * _scaleY + _matrix[9] * z * _scaleZ);
            ret.z = (_matrix[2] * x * _scaleX + _matrix[6] * y * _scaleY + _matrix[10] * z * _scaleZ);
            return ret;
        }

        /// <summary>
		/// Transforms a direction vector from this object's local space to the game's global space. 
		/// This means that rotation and scaling is applied, but translation is not.
		/// </summary>
        public virtual Vector3 TransformDirection(Vector3 dir)
        {
            return TransformDirection(dir.x, dir.y, dir.z);
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														Rotation
        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the object's rotation in degrees.
        /// </summary>
        /// <value>
        /// The rotation.
        /// </value>
        public Quaternion rotation {
			get { return _rotation; }
			set
            {
                _rotation = value;
                _rotationMatrixIsUpToDate = false;
                _rotationUpdates = 0;
            }
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														UpdateRotationMatrix
        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Internal function to update the rotation matrix.
        /// </summary>
        private void UpdateRotationMatrix()
        {
            if (_rotationMatrixIsUpToDate) return;
            if (_rotationUpdates > 10)
            {
                _rotationUpdates = 0;
                _rotation.Normalize();
            }

            Vector3 iHat = _rotation.Left;
            _matrix[0] = iHat.x;
            _matrix[1] = iHat.y;
            _matrix[2] = iHat.z;

            Vector3 jHat = _rotation.Up;
            _matrix[4] = jHat.x;
            _matrix[5] = jHat.y;
            _matrix[6] = jHat.z;

            Vector3 kHat = _rotation.Forward;
            _matrix[8] = kHat.x;
            _matrix[9] = kHat.y;
            _matrix[10] = kHat.z;

            _rotationMatrixIsUpToDate = true;
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														Rotate()
        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Rotate the object in question by a quaternion.
        /// </summary>
        /// <param name='q'>
        /// Quaternion to rotate by.
        /// </param>
        public void Rotate(Quaternion q)
        {
            _rotation *= q;
            _rotationMatrixIsUpToDate = false;
            _rotationUpdates++;
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														LookTowards()
        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Rotate the object to face a certain direction.
        /// </summary>
        /// <param name='direction'>
        /// The direction, in local space, to point towards.
        /// </param>
        public void LookTowards(Vector3 direction)
        {
            _rotation = Quaternion.LookTowards(direction);
            _rotationMatrixIsUpToDate = false;
            _rotationUpdates = 0;
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														Move()
        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Move the object, based on its current rotation.
        /// </summary>
        /// <param name='stepX'>
        /// Step x.
        /// </param>
        /// <param name='stepY'>
        /// Step y.
        /// </param>
		/// <param name='stepZ'>
		/// Step y.
		/// </param>
        public void Move(float stepX, float stepY, float stepZ)
        {
            Vector3 step = TransformDirection(stepX, stepY, stepZ);
            _matrix[12] += step.x;
            _matrix[13] += step.y;
            _matrix[14] += step.z;
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														Translate()
        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Move the object, in world space. (Object rotation is ignored)
        /// </summary>
        /// <param name='stepX'>
        /// Step x.
        /// </param>
        /// <param name='stepY'>
        /// Step y.
        /// </param>
		/// <param name='stepZ'>
		/// Step z.
		/// </param>
        public void Translate(float stepX, float stepY, float stepZ)
        {
            _matrix[12] += stepX;
			_matrix[13] += stepY;
            _matrix[14] += stepZ;
        }

		//------------------------------------------------------------------------------------------------------------------------
		//														scaleX
		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Sets the object's x-axis scale
		/// </summary>
		/// <value>
		/// The scale x.
		/// </value>
		public float scaleX {
			get { return _scaleX; }
			set { _scaleX = value; }
		}
		
		//------------------------------------------------------------------------------------------------------------------------
		//														scaleY
		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Sets the object's y-axis scale
		/// </summary>
		/// <value>
		/// The scale y.
		/// </value>
		public float scaleY {
			get { return _scaleY; }
			set { _scaleY = value; }
		}


        //------------------------------------------------------------------------------------------------------------------------
        //														scaleZ
        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Sets the object's z-axis scale
        /// </summary>
        /// <value>
        /// The scale z.
        /// </value>
        public float scaleZ
        {
            get { return _scaleZ; }
            set { _scaleZ = value; }
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														scale
        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Sets the object's x-axis, y-axis and z-axis scale
        /// </summary>
        /// <value>
        /// The scale.
        /// </value>
        public float scale {
			set { 
				_scaleX = value;
				_scaleY = value; 
                _scaleZ = value;
			}
		}

        //------------------------------------------------------------------------------------------------------------------------
        //														scale
        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the object's x-axis, y-axis and z-axis scale
        /// </summary>
        /// <value>
        /// The scale.
        /// </value>
        public Vector3 scaleXYZ
        {
            set
            {
                _scaleX = value.x;
                _scaleY = value.y;
                _scaleZ = value.z;
            }
            get { return new Vector3(_scaleX, _scaleY, _scaleZ); }
        }

        /// <summary>
        /// Returns the inverse matrix transformation, if it exists.
        /// (Use this e.g. for cameras used by sub windows)
        /// </summary>
        public Transformable Inverse() {
			if (scaleX == 0 || scaleY == 0 || scaleZ == 0)
				throw new Exception ("Cannot invert a transform with scale 0");
			Transformable inv=new Transformable();
            Vector3 localTranslation = InverseTransformPoint(0, 0, 0);
            inv.rotation = _rotation.Inverse();
            inv.scaleX = 1/_scaleX;
            inv.scaleY = 1/_scaleY;
            inv.scaleZ = 1/_scaleZ;
            inv.position = localTranslation;
            return inv;
		}
	}
}


