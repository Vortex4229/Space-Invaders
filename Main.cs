using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SpriteBatch = Microsoft.Xna.Framework.Graphics.SpriteBatch;

namespace Space_Invaders;

public class Main : Game {
	private bool _down;
	private int _downUnlockFrame;
	private int _enemyLaserCount;
	private List<Enemy> _enemyList;
	private int _enemySpeed;
	private List<Explosion> _explosionList;
	private int _frames;
	private bool _gameOver;
	private bool _justDied;
	private bool _justSpedUp;
	private int _lives;
	private bool _mainMenu;
	private Player _player;
	private bool _playerDead;
	private bool _playerInvincible;
	private int _playerInvincibleEndFrame;
	private int _playerReviveFrame;
	private int _score;
	private SpriteFont _scoreFont;
	private List<Shield> _shieldList;
	private SpriteBatch _spriteBatch;
	private SpriteFont _titleFont;
	private Ufo _ufo;

	public Main() {
		var graphics = new GraphicsDeviceManager(this);
		graphics.PreferredBackBufferWidth = 1600;
		graphics.PreferredBackBufferHeight = 1000;
		Content.RootDirectory = "Content";
		IsMouseVisible = true;
	}

	// rows begin 135 pixels down, columns begin at 313. each sprite is 65x65 with 26 pixels of space between (91 pixels total).
	private List<Enemy> EnemyGrid() {
		var enemyList = new List<Enemy>();
		for (var i = 0; i < 5; i++)
		for (var j = 0; j < 11; j++)
			if (i == 0)
				enemyList.Add(new Enemy(this, 3, new Vector2(313 + 91 * j, 125)));
			else if (i == 1 || i == 2)
				enemyList.Add(new Enemy(this, 2, new Vector2(313 + 91 * j, 125 + 91 * i)));
			else if (i == 3 || i == 4) enemyList.Add(new Enemy(this, 1, new Vector2(313 + 91 * j, 125 + 91 * i)));

		return enemyList;
	}

	private List<Shield> ConstructShields() {
		var shieldList = new List<Shield>();
		for (var i = 0; i < 4; i++) {
			shieldList.Add(new Shield(this, new Vector2(192 + 352 * i, 810)));
			shieldList.Add(new Shield(this, new Vector2(192 + 352 * i, 770)));
			shieldList.Add(new Shield(this, new Vector2(192 + 352 * i, 730), 'a'));
			shieldList.Add(new Shield(this, new Vector2(232 + 352 * i, 730)));
			shieldList.Add(new Shield(this, new Vector2(272 + 352 * i, 730)));
			shieldList.Add(new Shield(this, new Vector2(312 + 352 * i, 730), 'b'));
			shieldList.Add(new Shield(this, new Vector2(312 + 352 * i, 770)));
			shieldList.Add(new Shield(this, new Vector2(312 + 352 * i, 810)));
			shieldList.Add(new Shield(this, new Vector2(232 + 352 * i, 770), 'c'));
			shieldList.Add(new Shield(this, new Vector2(272 + 352 * i, 770), 'd'));
		}

		return shieldList;
	}

	protected override void Initialize() {
		_mainMenu = true;
		base.Initialize();
	}

	private void GameStart() {
		_mainMenu = false;
		_gameOver = false;
		_player = new Player(this);
		_enemyList = EnemyGrid();
		_explosionList = new List<Explosion>();
		_ufo = null;
		_shieldList = ConstructShields();
		_enemyLaserCount = 0;
		_frames = 0;
		_score = 0;
		_lives = 3;
		_enemySpeed = 40;
		_down = false;
		_downUnlockFrame = 0;
		_playerDead = false;
		_justDied = false;
		_justSpedUp = false;
		_playerReviveFrame = 0;
		_playerInvincible = false;
		_playerInvincibleEndFrame = 0;
	}

	protected override void LoadContent() {
		_spriteBatch = new SpriteBatch(GraphicsDevice);
		_scoreFont = Content.Load<SpriteFont>("pressStart2P");
		_titleFont = Content.Load<SpriteFont>("pressStart2PTitle");
	}

	protected override void Update(GameTime gameTime) {
		if (Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();
		if (_mainMenu || _gameOver)
			if (Keyboard.GetState().IsKeyDown(Keys.Enter))
				GameStart();
		if (!_gameOver && !_mainMenu) {
			//player logic
			if (_player != null) {
				if (_frames == _playerInvincibleEndFrame)
					_playerInvincible = false;
				_player.Update(gameTime);
				(_enemyList, _shieldList, _explosionList, _score, _justSpedUp) =
					_player.LaserCollisionCheck(_enemyList, _shieldList, _explosionList, _score, _frames, _ufo,
						_justSpedUp);
			}
			else if (_playerDead && _frames == _playerReviveFrame) {
				_player = new Player(this);
				_playerDead = false;
				_justDied = false;
				_playerInvincible = true;
				_playerInvincibleEndFrame = _frames + 60;
			}

			// enemy logic

			if (!_justSpedUp && _enemyList.Count != 55 && _enemyList.Count % 11 == 0) {
				_enemySpeed -= 4;
				_justSpedUp = true;
			}

			foreach (var enemy in _enemyList) {
				if ((enemy.Position.X <= 28f || enemy.Position.X + 65 >= 1575f) && _frames % _enemySpeed == 0 &&
				    _frames >= _downUnlockFrame) {
					_down = true;
					_downUnlockFrame = _frames + 50;
					break;
				}

				if (enemy.Position.Y + 65 >= 730) {
					_gameOver = true;
					break;
				}
			}

			if (_frames % _enemySpeed != 0) _down = false;

			foreach (var enemy in _enemyList) {
				_enemyLaserCount = enemy.Update(gameTime, _frames, _enemySpeed, _enemyLaserCount, _down);
				if (_player != null)
					(_player, _playerDead, _lives, _enemyLaserCount, _shieldList, _explosionList) =
						enemy.LaserCollisionCheck(_player, _lives, _enemyLaserCount, _frames, _shieldList,
							_explosionList, _playerInvincible);
				else
					(_, _, _lives, _enemyLaserCount, _shieldList, _explosionList) =
						enemy.LaserCollisionCheck(_player, _lives, _enemyLaserCount, _frames, _shieldList,
							_explosionList, _playerInvincible);
			}

			if (_playerDead && !_justDied) {
				_playerReviveFrame = _frames + 15;
				_justDied = true;
			}

			if (_enemyList.Count == 0 && _explosionList.Count == 0) {
				_enemyList = EnemyGrid();
				_lives++;
				_enemySpeed = 40;
			}

			// ufo logic
			if (_frames != 0 && _frames % 1536 == 0 && _enemyList.Count > 6) _ufo = new Ufo(this);

			if (_ufo != null) {
				_ufo.Update(gameTime);
				if (_ufo.IsDead || _ufo.Position.X >= 1600) _ufo = null;
			}

			// explosion logic
			for (var i = 0; i < _explosionList.Count; i++) {
				_explosionList[i].Update(_frames);
				if (_explosionList[i].Finished) _explosionList.RemoveAt(i);
			}

			// other logic
			if (_lives == 0 && _explosionList.Count == 0) _gameOver = true;
			_frames++;
		}

		base.Update(gameTime);
	}

	protected override void Draw(GameTime gameTime) {
		GraphicsDevice.Clear(Color.Black);
		_spriteBatch.Begin();

		if (_mainMenu) {
			_spriteBatch.DrawString(_titleFont, "SPACE INVADERS",
				new Vector2(800 - 0.5f * _titleFont.MeasureString("SPACE INVADERS").X,
					450 - 0.5f * _titleFont.MeasureString("SPACE INVADERS").Y), Color.White);
			_spriteBatch.DrawString(_scoreFont, "Press Enter To Start",
				new Vector2(800 - 0.5f * _scoreFont.MeasureString("Press Enter To Start").X,
					550 - 0.5f * _titleFont.MeasureString("Press Enter To Start").Y), Color.White);
			_spriteBatch.DrawString(_scoreFont, "Made by Paulo Bello",
				new Vector2(10, 990 - _scoreFont.MeasureString("Made by Paulo Bello").Y), Color.White);
			_spriteBatch.DrawString(_scoreFont, "v1.0",
				new Vector2(1590 - _scoreFont.MeasureString("v1.0").X, 990 - _scoreFont.MeasureString("v1.0").Y),
				Color.White);
		}

		if (!_gameOver && !_mainMenu) {
			var scoreText = $"Score {_score}";
			var livesText = $"Lives {_lives}";
			_spriteBatch.DrawString(_scoreFont, scoreText, new Vector2(10, 10), Color.White);
			_spriteBatch.DrawString(_scoreFont, livesText, new Vector2(1590 - _scoreFont.MeasureString(livesText).X, 10), Color.White);
			if (_player != null) _player.Draw(gameTime, _spriteBatch);
			foreach (var enemy in _enemyList) enemy.Draw(gameTime, _spriteBatch);
			if (_ufo != null) _ufo.Draw(gameTime, _spriteBatch);
			foreach (var shield in _shieldList) shield.Draw(gameTime, _spriteBatch);
			foreach (var explosion in _explosionList) explosion.Draw(_spriteBatch);
		}
		else if (_gameOver) {
			var scoreText = $"Score {_score}";
			var livesText = $"Lives {_lives}";
			_spriteBatch.DrawString(_scoreFont, scoreText, new Vector2(10, 10), Color.White);
			_spriteBatch.DrawString(_scoreFont, livesText, new Vector2(1590 - _scoreFont.MeasureString(livesText).X, 10), Color.White);
			_spriteBatch.DrawString(_titleFont, "GAME OVER",
				new Vector2(800 - 0.5f * _titleFont.MeasureString("GAME OVER").X,
					450 - 0.5f * _titleFont.MeasureString("GAME OVER").Y), Color.White);
			_spriteBatch.DrawString(_scoreFont, "Press Enter To Restart",
				new Vector2(800 - 0.5f * _scoreFont.MeasureString("Press Enter To Restart").X,
					550 - 0.5f * _titleFont.MeasureString("Press Enter To Restart").Y), Color.White);
		}

		_spriteBatch.End();

		base.Draw(gameTime);
	}
}