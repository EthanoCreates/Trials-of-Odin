using Ami.BroAudio;
using RootMotion.FinalIK;
using System;
using TrialsOfOdin.Combat;
using TrialsOfOdin.State;
using UnityEngine;

namespace TrialsOfOdin
{
    public class PlayerSoundManager : MonoBehaviour
    {
        [SerializeField] private GrounderFBBIK footstepFBBIK;

        [SerializeField] private SoundID hurtSounds;

        //Footstep sounds
        [SerializeField] private SoundID footstepSounds;
        [SerializeField] private SoundID snowFootstepSounds;

        [SerializeField] private SoundID runSounds;
        [SerializeField] private SoundID snowRunSounds;

        [SerializeField] private SoundID jumpSound;
        [SerializeField] private SoundID landSound;
        [SerializeField] private SoundID rollSound;

        //using final IKs grounder to figure out if an animations
        //foot positioning is entering a footstep position
        private int snowLayerMask = (1 << 17);

        private void Start()
        {
            footstepFBBIK.solver.OnInitiatedLegs += Solver_OnInitiatedLegs;
            SoundRequestor soundRequestor = PlayerStateMachine.LocalInstance.SoundRequestor;
            soundRequestor.OnJumpSound += AudioRequestor_OnJumpSound;
            soundRequestor.OnLandSound += AudioRequestor_OnLandSound;
            soundRequestor.OnRollLand += SoundRequestor_OnRollLand;
            soundRequestor.OnHurt += SoundRequestor_OnHurt;
        }

        private void SoundRequestor_OnHurt()
        {
            hurtSounds.Play();
        }

        private void SoundRequestor_OnRollLand()
        {
            rollSound.Play();
        }

        private void AudioRequestor_OnLandSound()
        {
            landSound.Play();
        }

        private void AudioRequestor_OnJumpSound()
        {
            jumpSound.Play();
        }

        private void PlayerAnimationEvents_OnWeaponEffortSound(object sender, EventArgs e)
        {
            Weapon weapon = PlayerStateMachine.LocalInstance.WeaponHolster.ActiveWeapon;
            weapon.WeaponData.effortAsset.MulticlipsPlayMode = Ami.BroAudio.Data.MulticlipsPlayMode.Velocity;
            weapon.WeaponData.effortSounds.Play().SetVelocity(weapon.currentAttack.effortVelocity);
        }

        private void Solver_OnInitiatedLegs(object sender, EventArgs e)
        {
            footstepFBBIK.solver.legs[0].OnStep += Leg_OnFootStep;
            footstepFBBIK.solver.legs[1].OnStep += Leg_OnFootStep;
        }

        //Playing sounds depending on ground type
        private void Leg_OnFootStep(object sender, EventArgs e)
        {
            if (PlayerStateMachine.LocalInstance.currentState.CurrentSubState.StateKey == ECharacterState.Run)
            {
                if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, 1.5f, snowLayerMask))
                {
                    snowRunSounds.Play();
                }
                else
                {
                    runSounds.Play();
                }
            }
            else
            {
                if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, 1.5f, snowLayerMask))
                {
                    snowFootstepSounds.Play();
                }
                else
                {
                    footstepSounds.Play();
                }
            }
        }
    }
}