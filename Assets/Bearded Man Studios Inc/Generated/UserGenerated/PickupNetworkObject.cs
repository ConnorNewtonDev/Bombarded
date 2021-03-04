using BeardedManStudios.Forge.Networking.Frame;
using BeardedManStudios.Forge.Networking.Unity;
using System;
using UnityEngine;

namespace BeardedManStudios.Forge.Networking.Generated
{
	[GeneratedInterpol("{\"inter\":[0]")]
	public partial class PickupNetworkObject : NetworkObject
	{
		public const int IDENTITY = 1;

		private byte[] _dirtyFields = new byte[1];

		#pragma warning disable 0067
		public event FieldChangedEvent fieldAltered;
		#pragma warning restore 0067
		[ForgeGeneratedField]
		private int _pickupType;
		public event FieldEvent<int> pickupTypeChanged;
		public Interpolated<int> pickupTypeInterpolation = new Interpolated<int>() { LerpT = 0f, Enabled = false };
		public int pickupType
		{
			get { return _pickupType; }
			set
			{
				// Don't do anything if the value is the same
				if (_pickupType == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[0] |= 0x1;
				_pickupType = value;
				hasDirtyFields = true;
			}
		}

		public void SetpickupTypeDirty()
		{
			_dirtyFields[0] |= 0x1;
			hasDirtyFields = true;
		}

		private void RunChange_pickupType(ulong timestep)
		{
			if (pickupTypeChanged != null) pickupTypeChanged(_pickupType, timestep);
			if (fieldAltered != null) fieldAltered("pickupType", _pickupType, timestep);
		}

		protected override void OwnershipChanged()
		{
			base.OwnershipChanged();
			SnapInterpolations();
		}
		
		public void SnapInterpolations()
		{
			pickupTypeInterpolation.current = pickupTypeInterpolation.target;
		}

		public override int UniqueIdentity { get { return IDENTITY; } }

		protected override BMSByte WritePayload(BMSByte data)
		{
			UnityObjectMapper.Instance.MapBytes(data, _pickupType);

			return data;
		}

		protected override void ReadPayload(BMSByte payload, ulong timestep)
		{
			_pickupType = UnityObjectMapper.Instance.Map<int>(payload);
			pickupTypeInterpolation.current = _pickupType;
			pickupTypeInterpolation.target = _pickupType;
			RunChange_pickupType(timestep);
		}

		protected override BMSByte SerializeDirtyFields()
		{
			dirtyFieldsData.Clear();
			dirtyFieldsData.Append(_dirtyFields);

			if ((0x1 & _dirtyFields[0]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _pickupType);

			// Reset all the dirty fields
			for (int i = 0; i < _dirtyFields.Length; i++)
				_dirtyFields[i] = 0;

			return dirtyFieldsData;
		}

		protected override void ReadDirtyFields(BMSByte data, ulong timestep)
		{
			if (readDirtyFlags == null)
				Initialize();

			Buffer.BlockCopy(data.byteArr, data.StartIndex(), readDirtyFlags, 0, readDirtyFlags.Length);
			data.MoveStartIndex(readDirtyFlags.Length);

			if ((0x1 & readDirtyFlags[0]) != 0)
			{
				if (pickupTypeInterpolation.Enabled)
				{
					pickupTypeInterpolation.target = UnityObjectMapper.Instance.Map<int>(data);
					pickupTypeInterpolation.Timestep = timestep;
				}
				else
				{
					_pickupType = UnityObjectMapper.Instance.Map<int>(data);
					RunChange_pickupType(timestep);
				}
			}
		}

		public override void InterpolateUpdate()
		{
			if (IsOwner)
				return;

			if (pickupTypeInterpolation.Enabled && !pickupTypeInterpolation.current.UnityNear(pickupTypeInterpolation.target, 0.0015f))
			{
				_pickupType = (int)pickupTypeInterpolation.Interpolate();
				//RunChange_pickupType(pickupTypeInterpolation.Timestep);
			}
		}

		private void Initialize()
		{
			if (readDirtyFlags == null)
				readDirtyFlags = new byte[1];

		}

		public PickupNetworkObject() : base() { Initialize(); }
		public PickupNetworkObject(NetWorker networker, INetworkBehavior networkBehavior = null, int createCode = 0, byte[] metadata = null) : base(networker, networkBehavior, createCode, metadata) { Initialize(); }
		public PickupNetworkObject(NetWorker networker, uint serverId, FrameStream frame) : base(networker, serverId, frame) { Initialize(); }

		// DO NOT TOUCH, THIS GETS GENERATED PLEASE EXTEND THIS CLASS IF YOU WISH TO HAVE CUSTOM CODE ADDITIONS
	}
}
