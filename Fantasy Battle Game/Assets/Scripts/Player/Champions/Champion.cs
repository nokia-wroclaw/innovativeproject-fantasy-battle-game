using System.Collections;
using BattleManagement;
using Champions.ChampionsUtilities;
using Map;
using Champions.CharacterUtilities.Movements;
using UnityEngine;
using Player;
using UnityEngine.EventSystems;

namespace Champions
{
    public class Champion : HexMovement
    {
        public GameObject GameObject { get; set; }
        private Player.Player owner_;
        private bool hasAniComp = false;
        //stats
        public int MaxHp;
        public int CurrentHp;
        public int Damage;
        public int Range = 10;
        private AnimationController animationController_;
        private ChampionsManager championsManager_;

        void Awake()
        {
            CurrentHp = MaxHp;
            if (null != GetComponent<Animation>())
            {
                hasAniComp = true;
            }
            animationController_ = AnimationController.Instance;
            championsManager_ = ChampionsManager.Instance;
        }
        
        public void AddDamagePopup(int damage)
        {
            var go = Instantiate(GridMetrics.Instance.DamagePopup, transform.position, Quaternion.identity);
            go.transform.eulerAngles = new Vector3(30, 0, 0);
            go.GetComponent<TextMesh>().text = "-" + damage.ToString();
        }

        public void PhysicalAttack(Champion defenderChampion, Tile destinationTile)
        {   
            TurnManagement.Instance.turnPhase_ = TurnPhase.MovingPhase;
            DestinationTile = destinationTile;
        
            StartCoroutine(physicalAttack(defenderChampion, destinationTile));
            CurrentPossition = destinationTile;
            
            defenderChampion.CurrentHp -= Damage;

        }

        IEnumerator physicalAttack(Champion defenderChampion, Tile destinationTile)
        {
            if (DestinationTile != currentTile)
            {
                yield return Move();
            }
            yield return LookAtIEnumerator(defenderChampion.CurrentPossition.Position);
            TurnManagement.Instance.turnPhase_ = TurnPhase.AttackPhase;
       
            defenderChampion.AddDamagePopup(Damage);

            if (defenderChampion.CurrentHp<=0)
            {
                if (championsManager_.SelectedChampion != null)
                    animationController_.DeadAnimation(championsManager_.SelectedChampion);
                defenderChampion.Destroy();
            }

            var x = defenderChampion.GameObject.GetComponent<DamagePopup>();
            
                
            if (championsManager_.SelectedChampion != null)
                animationController_.AttackAnimation(championsManager_.SelectedChampion);
            animationController_.DamageAnimation(defenderChampion);
            animationController_.RestAnimation(defenderChampion);
            Debug.Log(defenderChampion.CurrentHp+"/"+MaxHp);
            yield return new WaitForSeconds(2);
            TurnManagement.Instance.NextTurn();
        }

        public Tile CurrentPossition
        {
            get { return currentTile; }
            set
            {
                if (currentTile)
                {
                    currentTile.Champion = null;
                    currentTile.Available = true;
                }
                currentTile = value;
                value.Champion = this;
                value.Available = false;
            }
        }


        public Player.Player Owner
        {
            get { return owner_; }
            set
            {
                owner_ = value;
                value.Champions.Add(this);
            }
        }
        
        public void Destroy()
        {
            if (championsManager_.SelectedChampion != null)
                animationController_.DeadAnimation(championsManager_.SelectedChampion);
            Debug.Log("destroy champion");
            owner_.Champions.Remove(this);
            CurrentPossition.DeleteChildsGO();
            Destroy(GameObject);
            CurrentPossition.Champion = null;
            CurrentPossition.Available = true;
        }

        public void AddProjector(GameObject projector)
        {
            var newProjector = Instantiate(projector);
            newProjector.transform.parent = transform;
            newProjector.transform.position = projector.transform.position + transform.position;
        }
    }
}
