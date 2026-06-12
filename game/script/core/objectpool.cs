using System;
using System.Collections.Generic;
using purge_v0_4_0.game.script.entities;

namespace purge_v0_4_0.game.script.core
{
    public class objectpool
    {
        private gameconfig _config;

        // 对象池
        private queue<bullet> _bulletpool;
        private queue<enemy> _enemypool;
        private queue<feastbullet> _feastbulletpool;
        private queue<throwablefield> _effectpool;

        // 活动对象计数
        private int _activebullets = 0;
        private int _activeenemies = 0;
        private int _activeeffects = 0;

        // 上限
        private int _maxbullets;
        private int _maxenemies;
        private int _maxeffects;

        public objectpool(gameconfig config)
        {
            _config = config;
            _maxbullets = config.max_bulletpool;
            _maxenemies = config.max_enemypool;
            _maxeffects = config.max_effectpool;

            _bulletpool = new queue<bullet>();
            _enemypool = new queue<enemy>();
            _feastbulletpool = new queue<feastbullet>();
            _effectpool = new queue<throwablefield>();

            // 预创建对象
            for (int i = 0; i < 100; i++)
            {
                _bulletpool.enqueue(new bullet());
                _feastbulletpool.enqueue(new feastbullet());
            }

            for (int i = 0; i < 50; i++)
            {
                _enemypool.enqueue(new enemy());
            }
        }

        public bullet getbullet()
        {
            if (_bulletpool.count > 0)
            {
                var bullet = _bulletpool.dequeue();
                bullet.reset();
                _activebullets++;
                return bullet;
            }

            if (_activebullets < _maxbullets)
            {
                _activebullets++;
                return new bullet();
            }

            return null;
        }

        public feastbullet getfeastbullet()
        {
            if (_feastbulletpool.count > 0)
            {
                var bullet = _feastbulletpool.dequeue();
                bullet.reset();
                _activebullets++;
                return bullet;
            }

            if (_activebullets < _maxbullets)
            {
                _activebullets++;
                return new feastbullet();
            }

            return null;
        }

        public void returnbullet(bullet bullet)
        {
            if (bullet is feastbullet feast)
            {
                if (_feastbulletpool.count < _maxbullets / 2)
                {
                    _feastbulletpool.enqueue(feast);
                }
            }
            else
            {
                if (_bulletpool.count < _maxbullets / 2)
                {
                    _bulletpool.enqueue(bullet);
                }
            }
            _activebullets--;
        }

        public enemy getenemy(string type, float x, float y, float healthmult, float speedmult)
        {
            enemy enemy = null;

            if (_enemypool.count > 0)
            {
                enemy = _enemypool.dequeue();
            }
            else if (_activeenemies < _maxenemies)
            {
                enemy = new enemy();
            }

            if (enemy != null)
            {
                enemy.reset();
                enemy.initialize(type, x, y, healthmult, speedmult);
                _activeenemies++;
            }

            return enemy;
        }

        public void returnenemy(enemy enemy)
        {
            if (_enemypool.count < _maxenemies / 2)
            {
                _enemypool.enqueue(enemy);
            }
            _activeenemies--;
        }

        public throwablefield geteffectfield()
        {
            if (_effectpool.count > 0)
            {
                var effect = _effectpool.dequeue();
                effect.reset();
                _activeeffects++;
                return effect;
            }

            if (_activeeffects < _maxeffects)
            {
                _activeeffects++;
                return new throwablefield();
            }

            return null;
        }

        public void returneffectfield(throwablefield field)
        {
            if (_effectpool.count < _maxeffects / 2)
            {
                _effectpool.enqueue(field);
            }
            _activeeffects--;
        }

        public void setmaxbullets(int max)
        {
            _maxbullets = max;
            _config.max_bulletpool = max;
        }

        public void setmaxenemies(int max)
        {
            _maxenemies = max;
            _config.max_enemypool = max;
        }

        public int activebullets => _activebullets;
        public int activeenemies => _activeenemies;
        public int maxbullets => _maxbullets;
        public int maxenemies => _maxenemies;

        public void clearall()
        {
            _bulletpool.clear();
            _feastbulletpool.clear();
            _enemypool.clear();
            _effectpool.clear();
            _activebullets = 0;
            _activeenemies = 0;
            _activeeffects = 0;
        }
    }
}
