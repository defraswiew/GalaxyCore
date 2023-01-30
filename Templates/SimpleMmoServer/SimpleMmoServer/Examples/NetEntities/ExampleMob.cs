using System;
using System.Collections.Generic;
using System.Linq;
using GalaxyCoreCommon;
using GalaxyCoreServer;

namespace SimpleMmoServer.Examples.NetEntities
{
    public class ExampleMob:NetEntity
    {
        private GalaxyVector3[] _points;
        private GalaxyVector3 _currentPoint;
        private int _currentIndex;
        private float _speed = 1;
        private bool _move = true;

       
        
        public ExampleMob(Instance instance, GalaxyVector3 position = new GalaxyVector3(), GalaxyQuaternion rotation = new GalaxyQuaternion()) : base(instance, position, rotation)
        {
           
        }

        private void LoadPoints(IEnumerable<(float x, float y)> sourcePoints)
        {
            // делаем массив, для удобства работы
            var prePoints = sourcePoints.ToArray();
            // инициализируем массив пути (туда - обратно)
            _points = new GalaxyVector3[prePoints.Length * 2];
            var index = 0;
            // Заполняем путь туда
            foreach (var point in prePoints)
            {
                _points[index] = new GalaxyVector3(point.x, point.y, 0);
                index++;
            }

            // запоняем путь обратно
            prePoints.Reverse();
            foreach (var point in prePoints)
            {
                _points[index] = new GalaxyVector3(point.x, point.y, 0);
                index++;
            }

            SetNextPoint();
        }

        private void SetNextPoint()
        {
            if (_currentIndex >= _points.Length)
            {
                _currentIndex = 0;
            }
            _currentPoint = _points[_currentIndex];
            _currentIndex++;
        }

        protected override void Update()
        {
            if(!_move) return;
            if(_currentPoint.IsZero()) return;

            if ((_currentPoint - transform.Position).SqrMagnitude > 0.3f)
            {
                transform.Position = GalaxyVector3.Move(transform.Position, _currentPoint, _speed, Instance.Time.DeltaTime);
            }
            else
            {
                SetNextPoint();
            }
            
        }

        public override void InMessage(byte externalCode, byte[] data, BaseClient clientSender)
        {
             
        }

        protected override void OnRemotePosition(GalaxyVector3 remotePosition)
        {
             
        }

        protected override void OnRemoteScale(GalaxyVector3 remoteScale)
        {
            
        }

        protected override void OnRemoteRotation(GalaxyQuaternion remoteRotation)
        {
            
        }

        protected override void Start()
        {
             
        }

        protected override void OnDestroy()
        {
          
        }
    }
}