using BeardedManStudios.Forge.Networking.Frame;
using BeardedManStudios.Forge.Networking.Unity;
using System;
using UnityEngine;

namespace BeardedManStudios.Forge.Networking.Generated
{
	[GeneratedInterpol("{\"inter\":[0]")]
	public partial class LobbyNetworkObject : NetworkObject
	{
		public const int IDENTITY = 2;

		private byte[] _dirtyFields = new byte[1];

		#pragma warning disable 0067
		public event FieldChangedEvent fieldAltered;
		#pragma warning restore 0067
		[ForgeGeneratedField]
		private int _gameState;
		public event FieldEvent<int> gameStateChanged;
		public Interpolated<int> gameStateInterpolation = new Interpolated<int>() { LerpT = 0f, Enabled = false };
		public int gameState
		{
			get { return _gameState; }
			set
			{
				// Don't do anything if the value is the same
				if (_gameState == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[0] |= 0x1;
				_gameState = value;
				hasDirtyFields = true;
			}
		}

		public void SetgameStateDirty()
		{
			_dirtyFields[0] |= 0x1;
			hasDirtyFields = true;
		}

		private void RunChange_gameState(ulong timestep)
		{
			if (gameStateChanged != null) gameStateChanged(_gameState, timestep);
			if (fieldAltered != null) fieldAltered("gameState", _gameState, timestep);
		}

		protected override void OwnershipChanged()
		{
			base.OwnershipChanged();
			SnapInterpolations();
		}
		
		public void SnapInterpolations()
		{
			gameStateInterpolation.current = gameStateInterpolation.target;
		}

		public override int UniqueIdentity { get { return IDENTITY; } }

		protected override BMSByte WritePayload(BMSByte data)
		{
			UnityObjectMapper.Instance.MapBytes(data, _gameState);

			return data;
		}

		protected override void ReadPayload(BMSByte payload, ulong timestep)
		{
			_gameState = UnityObjectMapper.Instance.Map<int>(payload);
			gameStateInterpolation.current = _gameState;
			gameStateInterpolation.target = _gameState;
			RunChange_gameState(timestep);
		}

		protected override BMSByte SerializeDirtyFields()
		{
			dirtyFieldsData.Clear();
			dirtyFieldsData.Append(_dirtyFields);

			if ((0x1 & _dirtyFields[0]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _gameState);

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
				if (gameStateInterpolation.Enabled)
				{
					gameStateInterpolation.target = UnityObjectMapper.Instance.Map<int>(data);
					gameStateInterpolation.Timestep = timestep;
				}
				else
				{
					_gameState = UnityObjectMapper.Instance.Map<int>(data);
					RunChange_gameState(timestep);
				}
			}
		}

		public override void InterpolateUpdate()
		{
			if (IsOwner)
				return;

			if (gameStateInterpolation.Enabled && !gameStateInterpolation.current.UnityNear(gameStateInterpolation.target, 0.0015f))
			{
				_gameState = (int)gameStateInterpolation.Interpolate();
				//RunChange_gameState(gameStateInterpolation.Timestep);
			}
		}

		private void Initialize()
		{
			if (readDirtyFlags == null)
				readDirtyFlags = new byte[1];

		}

		public LobbyNetworkObject() : base() { Initialize(); }
		public LobbyNetworkObject(NetWorker networker, INetworkBehavior networkBehavior = null, int createCode = 0, byte[] metadata = null) : base(networker, networkBehavior, createCode, metadata) { Initialize(); }
		public LobbyNetworkObject(NetWorker networker, uint serverId, FrameStream frame) : base(networker, serverId, frame) { Initialize(); }

		// DO NOT TOUCH, THIS GETS GENERATED PLEASE EXTEND THIS CLASS IF YOU WISH TO HAVE CUSTOM CODE ADDITIONS
	}
}
