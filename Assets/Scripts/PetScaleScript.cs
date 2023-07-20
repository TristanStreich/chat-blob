using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetScaleScript : MonoBehaviour
{

    public SpringJoint2D[] Centers;
    public SpringJoint2D[] Ones;
    public SpringJoint2D[] Twos;
    public SpringJoint2D[] Threes;

    private float CenterChange = 0.2f;
    private float OnesChange = 0.23f;
    private float TwosChange = 0.37f;
    private float ThreesChange = 0.42f;

    private float springFreqChange = 0.5f;

    [ContextMenu("Grow")]
    public void Grow()
    {
        if (gameObject.transform.localScale.x <= 0.4f)
        {
            //change size
            gameObject.transform.localScale += new Vector3(0.1f, 0.1f, 0);

            //Set spring distances and freq
            foreach (SpringJoint2D Spring in Centers)
            {
                Spring.distance += CenterChange;
                Spring.frequency -= springFreqChange;
            }
            foreach (SpringJoint2D Spring in Ones)
            {
                Spring.distance += OnesChange;
                Spring.frequency -= springFreqChange;
            }
            foreach (SpringJoint2D Spring in Twos)
            {
                Spring.distance += TwosChange;
                Spring.frequency -= springFreqChange;
            }
            foreach (SpringJoint2D Spring in Threes)
            {
                Spring.distance += ThreesChange;
                Spring.frequency -= springFreqChange;
            }

            //change other things that need to scale
            PetBehavior.PetBehav.raycastDistance += 0.1f;
        }
    }

    [ContextMenu("Shrink")]
    public void Shrink()
    {
        if (gameObject.transform.localScale.x >= 0.2f)
        {
            //change size
            gameObject.transform.localScale -= new Vector3(0.1f, 0.1f, 0);

            //Set spring distances and freq
            foreach (SpringJoint2D Spring in Centers)
            {
                Spring.distance -= CenterChange;
                Spring.frequency += springFreqChange;
            }
            foreach (SpringJoint2D Spring in Ones)
            {
                Spring.distance -= OnesChange;
                Spring.frequency += springFreqChange;
            }
            foreach (SpringJoint2D Spring in Twos)
            {
                Spring.distance -= TwosChange;
                Spring.frequency += springFreqChange;
            }
            foreach (SpringJoint2D Spring in Threes)
            {
                Spring.distance -= ThreesChange;
                Spring.frequency += springFreqChange;
            }

            //change other things that need to scale
            PetBehavior.PetBehav.raycastDistance += -0.1f;
        }
    }
}
