using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Space_Invaders;

public class Player {
	private readonly Main _root;
	private readonly Texture2D _sprite;
	private Laser _laser;
	private Vector2 _position;


	public Player(Main root) {
		_root = root;
		_sprite = _root.Content.Load<Texture2D>("playerSprite");
		_position = new Vector2(725, 910);
		_laser = null;
		Hitbox = new Rectangle((int)_position.X, (int)_position.Y, _sprite.Width, _sprite.Height);
	}


	public Vector2 Position => _position;

	public Rectangle Hitbox { get; private set; }

	private void Shoot() {
		if (_laser != null) return; // in the original game, there can only be one player laser on screen at a time.
		_laser = new Laser(_root, new Vector2(_position.X + (float)_sprite.Width / 2, _position.Y - 9), 1);
	}

	public void Update(GameTime gameTime) {
		if (Keyboard.GetState().IsKeyDown(Keys.D) && _position.X + _sprite.Width <= 1600) _position.X += 6;
		if (Keyboard.GetState().IsKeyDown(Keys.A) && _position.X >= 0) _position.X -= 6;
		if (Keyboard.GetState().IsKeyDown(Keys.Space)) Shoot();

		if (_laser != null) _laser.Update(gameTime);

		Hitbox = new Rectangle((int)_position.X, (int)_position.Y, _sprite.Width, _sprite.Height);
	}

	public (List<Enemy>, List<Shield>, List<Explosion>, int, bool) LaserCollisionCheck(List<Enemy> enemyList,
		List<Shield> shieldList, List<Explosion> explosionList, int score, int frames, Ufo ufo, bool justSpedUp) {
		if (_laser == null) return (enemyList, shieldList, explosionList, score, justSpedUp);
		if (_laser.Location.Y <= -23) {
			_laser = null;
			return (enemyList, shieldList, explosionList, score, justSpedUp);
		}

		for (var i = 0; i < enemyList.Count; i++)
			if (enemyList[i].Hitbox.Intersects(_laser.Hitbox)) {
				score += enemyList[i].GetValue();
				var explosionPosition = enemyList[i].Position;
				enemyList.RemoveAt(i);
				explosionList.Add(new Explosion(_root, explosionPosition, frames));
				_laser = null;
				return (enemyList, shieldList, explosionList, score, false);
			}

		for (var i = 0; i < shieldList.Count; i++)
			if (shieldList[i].Hitbox.Intersects(_laser.Hitbox)) {
				shieldList[i].Damage();
				if (shieldList[i].HealthPoints <= 0) shieldList.RemoveAt(i);
				_laser = null;
				return (enemyList, shieldList, explosionList, score, justSpedUp);
			}

		if (ufo != null)
			if (ufo.Hitbox.Intersects(_laser.Hitbox)) {
				score += ufo.GetScore();
				var explosionPosition = ufo.Position;
				explosionList.Add(new Explosion(_root, explosionPosition, frames));
				_laser = null;
			}

		return (enemyList, shieldList, explosionList, score, justSpedUp);
	}

	public void Draw(GameTime gameTime, SpriteBatch spriteBatch) {
		spriteBatch.Draw(_sprite, _position, Color.White);
		if (_laser != null) _laser.Draw(gameTime, spriteBatch);
	}
}