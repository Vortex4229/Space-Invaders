using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Space_Invaders;

public class Enemy {
	private readonly Random _rnd;
	private readonly Main _root;
	private readonly Texture2D _sprite1;
	private readonly Texture2D _sprite2;
	private readonly byte _type;
	private Texture2D _activeSprite;
	private int _cooldownEndFrame;
	private Laser _laser;
	private sbyte _movementDirection;
	private Vector2 _position;
	private sbyte _spriteSwitch;

	public Enemy(Main root, byte type, Vector2 position) {
		_root = root;
		_type = type;
		_sprite1 = _root.Content.Load<Texture2D>($"enemySprite{type}");
		_sprite2 = _root.Content.Load<Texture2D>($"enemySprite{type}a");
		_activeSprite = _sprite1;
		_spriteSwitch = -1; // -1 = sprite1, 1 = sprite2
		_position = position;
		_movementDirection = -1; // -1 = left, 1 = right
		Hitbox = new Rectangle((int)_position.X, (int)_position.Y, _activeSprite.Width, _activeSprite.Height);
		_laser = null;
		_cooldownEndFrame = 0;
		_rnd = new Random();
	}

	public Rectangle Hitbox { get; private set; }

	public Vector2 Position => _position;

	private void Shoot() {
		_laser = new Laser(_root, new Vector2(_position.X + (float)_activeSprite.Width / 2, _position.Y - 9), 2);
	}

	public int Update(GameTime gameTime, int frames, int enemySpeed, int laserCount, bool down) {
		// movement code
		if (frames % enemySpeed == 0 && down && _type != 4) {
			_position.Y += _activeSprite.Height / 2.4f;
			_movementDirection *= -1;
		}
		else if (frames % enemySpeed == 0 && _movementDirection == -1) {
			_position.X -= _activeSprite.Width / 4.8f;
			_spriteSwitch *= -1;
		}
		else if (frames % enemySpeed == 0 && _movementDirection == 1) {
			_position.X += _activeSprite.Width / 4.8f;
			_spriteSwitch *= -1;
		}

		switch (_spriteSwitch) {
			case -1:
				_activeSprite = _sprite1;
				break;
			case 1:
				_activeSprite = _sprite2;
				break;
		}

		Hitbox = new Rectangle((int)_position.X, (int)_position.Y, _activeSprite.Width, _activeSprite.Height);

		// shooting code
		if (_type == 3 && _laser == null && laserCount < 5 && frames > _cooldownEndFrame &&
		    _rnd.NextFloat(0.0f, 1.0f) * 100 <= 1f) {
			Shoot();
			laserCount++;
			_cooldownEndFrame = frames + 300;
		}

		_laser?.Update(gameTime);

		return laserCount;
	}

	public (Player, bool, int, int, List<Shield>, List<Explosion>) LaserCollisionCheck(Player player, int lives,
		int laserCount, int frames, List<Shield> shieldList, List<Explosion> explosionList, bool playerInvincible) {
		if (_laser == null) return (player, false, lives, laserCount, shieldList, explosionList);
		for (var i = 0; i < shieldList.Count; i++)
			if (shieldList[i].Hitbox.Intersects(_laser.Hitbox)) {
				laserCount--;
				shieldList[i].Damage();
				if (shieldList[i].HealthPoints <= 0) shieldList.RemoveAt(i);
				_laser = null;
				return (player, false, lives, laserCount, shieldList, explosionList);
			}

		if (player != null && player.Hitbox.Intersects(_laser.Hitbox) && !playerInvincible) {
			lives -= 1;
			laserCount--;
			explosionList.Add(new Explosion(_root, player.Position, frames));
			_laser = null;
			return (null, true, lives, laserCount, shieldList, explosionList);
		}

		if (_laser.Location.Y >= 1000) {
			laserCount--;
			_laser = null;
		}

		return (player, false, lives, laserCount, shieldList, explosionList);
	}


	public int GetValue() {
		switch (_type) {
			case 1:
				return 10;
			case 2:
				return 20;
			case 3:
				return 30;
		}

		return 0;
	}

	public void Draw(GameTime gameTime, SpriteBatch spriteBatch) {
		spriteBatch.Draw(_activeSprite, _position, Color.White);
		if (_laser != null) _laser.Draw(gameTime, spriteBatch);
	}
}