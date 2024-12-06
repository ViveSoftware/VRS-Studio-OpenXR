namespace Wave.Essence.Sample.Hand
{/*
	[CustomEditor(typeof(HandController))]
	public class HandControllerEditor : UnityEditor.Editor
	{
		Animator AnimatorReference;
		static string[] ReferenceMenu = new string[] { "Animator" };
		static string UsingReferenceItem = ReferenceMenu[0];
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			HandController script = (HandController)target;
			if (EditorGUILayout.DropdownButton(new GUIContent(UsingReferenceItem), FocusType.Keyboard))
			{
				GenericMenu menu = new GenericMenu();
				foreach (string item in ReferenceMenu)
				{
					if (!string.IsNullOrEmpty(item))
					{
						menu.AddItem(new GUIContent(item), (UsingReferenceItem == item), OnDropDownSelected, item);
					}
				}
				menu.ShowAsContext();
			}

			switch (UsingReferenceItem)
			{
				case "Animator":
					AnimatorReference = (Animator)EditorGUILayout.ObjectField(label: "Reference", AnimatorReference, typeof(Animator), true);
					break;
			}


			if (GUILayout.Button("Update", GUILayout.Height(40)))
			{
				Debug.Log("Modify Root!");
				Transform tmp;

				switch (UsingReferenceItem)
				{
					case "Animator":
						AnimatorReference = (Animator)EditorGUILayout.ObjectField(label: "Reference", AnimatorReference, typeof(Animator), true);
						#region AnimatorRef
						tmp = AnimatorReference.GetBoneTransform(HumanBodyBones.Hips);
						if (tmp)
						{
							script.hip = tmp.transform;
						}

						tmp = AnimatorReference.GetBoneTransform(HumanBodyBones.LeftUpperLeg);
						if (tmp)
						{
							script.leftThigh = tmp.transform;
						}
						tmp = AnimatorReference.GetBoneTransform(HumanBodyBones.LeftLowerLeg);
						if (tmp)
						{
							script.leftLeg = tmp.transform;
						}
						tmp = AnimatorReference.GetBoneTransform(HumanBodyBones.LeftFoot);
						if (tmp)
						{
							script.leftAnkle = tmp.transform;
						}
						tmp = AnimatorReference.GetBoneTransform(HumanBodyBones.LeftToes);
						if (tmp)
						{
							script.leftFoot = tmp.transform;
						}
						tmp = AnimatorReference.GetBoneTransform(HumanBodyBones.RightUpperLeg);
						if (tmp)
						{
							script.rightThigh = tmp.transform;
						}
						tmp = AnimatorReference.GetBoneTransform(HumanBodyBones.RightLowerLeg);
						if (tmp)
						{
							script.rightLeg = tmp.transform;
						}
						tmp = AnimatorReference.GetBoneTransform(HumanBodyBones.RightFoot);
						if (tmp)
						{
							script.rightAnkle = tmp.transform;
						}
						tmp = AnimatorReference.GetBoneTransform(HumanBodyBones.RightToes);
						if (tmp)
						{
							script.rightFoot = tmp.transform;
						}
						tmp = AnimatorReference.GetBoneTransform(HumanBodyBones.Spine);
						if (tmp)
						{
							script.spineLower = tmp.transform;
						}
						tmp = AnimatorReference.GetBoneTransform(HumanBodyBones.Chest);
						if (tmp)
						{
							script.spineHigh = tmp.transform;
						}
						tmp = AnimatorReference.GetBoneTransform(HumanBodyBones.UpperChest);
						if (tmp)
						{
							script.chest = tmp.transform;
						}
						tmp = AnimatorReference.GetBoneTransform(HumanBodyBones.Neck);
						if (tmp)
						{
							script.neck = tmp.transform;
						}
						tmp = AnimatorReference.GetBoneTransform(HumanBodyBones.Head);
						if (tmp)
						{
							script.head = tmp.transform;
						}
						tmp = AnimatorReference.GetBoneTransform(HumanBodyBones.LeftShoulder);
						if (tmp)
						{
							script.leftScapula = tmp.transform;
						}
						tmp = AnimatorReference.GetBoneTransform(HumanBodyBones.LeftUpperArm);
						if (tmp)
						{
							script.leftUpperarm = tmp.transform;
						}
						tmp = AnimatorReference.GetBoneTransform(HumanBodyBones.LeftLowerArm);
						if (tmp)
						{
							script.leftForearm = tmp.transform;
						}
						tmp = AnimatorReference.GetBoneTransform(HumanBodyBones.LeftHand);
						if (tmp)
						{
							script.leftHand = tmp.transform;
						}
						tmp = AnimatorReference.GetBoneTransform(HumanBodyBones.RightShoulder);
						if (tmp)
						{
							script.rightScapula = tmp.transform;
						}
						tmp = AnimatorReference.GetBoneTransform(HumanBodyBones.RightUpperArm);
						if (tmp)
						{
							script.rightUpperarm = tmp.transform;
						}
						tmp = AnimatorReference.GetBoneTransform(HumanBodyBones.RightLowerArm);
						if (tmp)
						{
							script.rightForearm = tmp.transform;
						}
						tmp = AnimatorReference.GetBoneTransform(HumanBodyBones.RightHand);
						if (tmp)
						{
							script.rightHand = tmp.transform;
						}
						#endregion
						break;
				}
			}
		}

		void OnDropDownSelected(object value)
		{
			UsingReferenceItem = value.ToString();
		}
	}
	*/
}
