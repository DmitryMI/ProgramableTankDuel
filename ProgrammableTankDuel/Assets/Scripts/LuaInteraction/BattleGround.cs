using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.LuaInteraction
{
    public class BattleGround
    {
        private readonly Tank _tank;

        public BattleGround(Tank tank)
        {
            _tank = tank;
        }
        public Obstacle[] GetObstacles()
        {
            GameObject[] obst = GameObject.FindGameObjectsWithTag("Obstacle");
            Obstacle[] res = new Obstacle[obst.Length ];
            int i = 0;
            for (; i < res.Length; i++)
            {
                res[i] = new Obstacle(
                    obst[i].GetComponent<IPlaceable>().Width,
                    obst[i].GetComponent<IPlaceable>().Height, 
                    obst[i]);
            }
            return res;
        }

        public VehicleInfo[] GetEnemies()
        {
            GameObject[] obst = GameObject.FindGameObjectsWithTag("Player");
            List<VehicleInfo> res = new List<VehicleInfo>();
            Color thisTeam = _tank.GetTeam();
            foreach (var el in obst)
            {
                Tank tank = el.GetComponent<Tank>();
                Color team = tank.GetTeam();
                if(team == Color.white)
                    continue;
                //Debug.Log("My tank: " + _tank.name + ". His tank: " + tank.name + ". My team: " + thisTeam + " his: " + team + ". Equal: " + thisTeam.Equals(team));
                if (!el.Equals(_tank.GameObject) && !thisTeam.Equals(team))
                {
                    IPlaceable placeable = el.GetComponent<IPlaceable>();
                    res.Add(new VehicleInfo(placeable.Width, placeable.Height, el));
                }
            }
            return res.ToArray();
        }

        public VehicleInfo[] GetAllies()
        {
            GameObject[] obst = GameObject.FindGameObjectsWithTag("Player");
            List<VehicleInfo> res = new List<VehicleInfo>();
            Color thisTeam = _tank.GetTeam();
            foreach (var el in obst)
            {
                Tank tank = el.GetComponent<Tank>();
                Color team = tank.GetTeam();
                if (!el.Equals(_tank.GameObject) && thisTeam.Equals(team))
                {
                    IPlaceable placeable = el.GetComponent<IPlaceable>();
                    res.Add(new VehicleInfo(placeable.Width, placeable.Height, el));
                }
            }
            return res.ToArray();
        }

        public ReleasedShell[] GetShells()
        {
            GameObject[] obst = GameObject.FindGameObjectsWithTag("Shell");
            ReleasedShell[] res = new ReleasedShell[obst.Length];
            int i = 0;
            for (; i < res.Length; i++)
            {
                res[i] = new ReleasedShell(obst[i]);
            }
            return res;
        }
    }
}
