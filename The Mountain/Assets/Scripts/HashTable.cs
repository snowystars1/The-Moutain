using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HashTable : MonoBehaviour {

    //PLAYER CHARACTER
    //States
    public static int initialJumpState = Animator.StringToHash("Base Layer.InitialJump");
    public static int jumpState = Animator.StringToHash("Base Layer.Jump");
    public static int motionState = Animator.StringToHash("Base Layer.Motion");
    public static int battleMotionState = Animator.StringToHash("Base Layer.Battle_Base.BattleMotion");
    public static int glideState = Animator.StringToHash("Base Layer.Gliding");
    public static int grappleShootState = Animator.StringToHash("Base Layer.GrappleShoot");
    public static int dodgeRollState = Animator.StringToHash("DodgeRoll");

    //Parameters
    public static int onGroundParam = Animator.StringToHash("onGround");
    public static int gravityParam = Animator.StringToHash("Gravity");
    public static int IdleAltParam = Animator.StringToHash("IdleAlt");
    public static int ComboParam = Animator.StringToHash("Combo");
    public static int ComboCountParam = Animator.StringToHash("ComboCount");
    public static int controllerXParam = Animator.StringToHash("ControllerInputX");
    public static int controllerYParam = Animator.StringToHash("ControllerInputY");
    public static int battleIdleTimerParam = Animator.StringToHash("BattleIdleTimer");
    public static int jumpBlendParam = Animator.StringToHash("JumpBlend");
    public static int glidingParam = Animator.StringToHash("Gliding");
    public static int grapplingParam = Animator.StringToHash("Grappling");
    public static int forwardParam = Animator.StringToHash("Forward");
    public static int turnParam = Animator.StringToHash("Turn");
    public static int dodgeParam = Animator.StringToHash("DodgeRoll");

    public static int enemyHealthParam = Animator.StringToHash("EnemyHealth");
    public static int enemyHealthRParam = Animator.StringToHash("EnemyHealthR");
    public static int enemyHealthLParam = Animator.StringToHash("EnemyHealthL");

    //First set of ground combo states
    public static int swordStab2State = Animator.StringToHash("Battle_SwordStab2");
    public static int swordSpin2State = Animator.StringToHash("Battle_SwordSpinFinisher2");
    public static int swordSwing2State = Animator.StringToHash("Battle_SwordSwing2");

    //First set of air combo states
    public static int airUpSwing1State = Animator.StringToHash("Battle_AirUpSwing1");
    public static int airDownSwing1State = Animator.StringToHash("Battle_AirDownSwing1");
    public static int airSlashFinisher1State = Animator.StringToHash("Battle_AirSlashFinisher1");

    //Transitions
    public static int glideToJump = Animator.StringToHash("Base Layer.Gliding -> Base Layer.Jump");
    public static int glideToMotion = Animator.StringToHash("Base Layer.Gliding -> Base Layer.Motion");
    public static int motionToDodge = Animator.StringToHash("Base Layer.Motion -> Base Layer.DodgeRoll");
    public static int dodgeToMotion = Animator.StringToHash("Base Layer.DodgeRoll -> Base Layer.Motion");

    //Glider States
    public static int foldIn = Animator.StringToHash("Base Layer.GliderFoldIn");
    public static int foldOut = Animator.StringToHash("Base Layer.GliderFoldOut");

    //Glider Parameters
    public static int glideGliderParam = Animator.StringToHash("Glide");

    //SLIMEBOSS
    //STATES
    public static int slimeBossRumbleState = Animator.StringToHash("Base Layer.SlimeBoss_Rumble");
    public static int slimeBossSuckState = Animator.StringToHash("Base Layer.SlimeBoss_Suck1");
    public static int slimeBossBlowState = Animator.StringToHash("Base Layer.SlimeBoss_Blow1");
    public static int slimeBossGlobChargeState = Animator.StringToHash("Base Layer.GlobVolleyChargeUp");
    public static int slimeBossDyingState = Animator.StringToHash("Base Layer.SlimeBossDying");

    //SLIMEBOSSLEFTARM
    //STATES
    public static int slimeBossLArmRumble = Animator.StringToHash("Base Layer.SlimeBossLeftArmRumble");
    public static int slimeBossLArmSuck = Animator.StringToHash("Base Layer.SlimeBossLeftArmSuck");
    public static int slimeBossLArmBlow = Animator.StringToHash("Base Layer.SlimeBossLeftArmBlow");
    public static int slimeBossLArmSwipe = Animator.StringToHash("Base Layer.SlimeBossLeftArmSwipe");
    public static int slimeBossLArmIdle = Animator.StringToHash("Base Layer.SlimeBossLeftArmIdle");
    public static int slimeBossLArmVulnerable = Animator.StringToHash("Base Layer.SlimeBossLeftArmVulnerable");
    public static int slimeBossLArmGlobCharge = Animator.StringToHash("Base Layer.LeftGlobVolleyChargeUp");
    public static int slimeBossLArmDying = Animator.StringToHash("Base Layer.SlimeBossLeftArmDying");


    //SLIMEBOSSRIGHTARM
    //STATES
    public static int slimeBossRArmRumble = Animator.StringToHash("Base Layer.SlimeBossRightArmRumble");
    public static int slimeBossRArmSuck = Animator.StringToHash("Base Layer.SlimeBossRightArmSuck");
    public static int slimeBossRArmBlow = Animator.StringToHash("Base Layer.SlimeBossRightArmBlow");
    public static int slimeBossRArmSwipe = Animator.StringToHash("Base Layer.SlimeBossRightArmSwipe");
    public static int slimeBossRArmIdle = Animator.StringToHash("Base Layer.SlimeBossRightArmIdle");
    public static int slimeBossRArmVulnerable = Animator.StringToHash("Base Layer.SlimeBossRightArmVulnerable");
    public static int slimeBossRArmGlobCharge = Animator.StringToHash("Base Layer.RightGlobVolleyChargeUp");
    public static int slimeBossRArmDying = Animator.StringToHash("Base Layer.SlimeBossRightArmDying");

    //BIGDOOR PARAMETER
    public static int bigDoorOpenParam = Animator.StringToHash("Open");

}
