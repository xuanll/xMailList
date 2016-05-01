//the project for test of Firewing Games using NGUI

using UnityEngine;
using System.Collections;

public class MailList : MonoBehaviour {

	GameObject delBtn;
	GameObject mailScrollView;

	GameObject markedBtn;

	GameObject progressBar;

	static int mailCount = 50;

	int newIndex = mailCount;

	static int viewableCount = 10;

	int shownCount = viewableCount;

	BetterList<GameObject> markedList = new BetterList<GameObject>();

	BetterList<MailItem> mailBoxList = new BetterList<MailItem>();

	int updateMailTime = 10;

	class MailItem
	{
		public string title;
		public bool marked;
		public bool isPureText;
		public float arriveTime = 0f;
	}

	void Update()
	{
		//simulate receive mail,
		//[bug]to do: there's the bug when add new mail to the list, it appears the third place. The CustomSort didn't work...
		if(Time.realtimeSinceStartup % updateMailTime < 0.01f)
		{
			Debug.Log("Update Mail...");
			Debug.Log("realtimeSinceStartup: " + Time.realtimeSinceStartup);


			float rTime = Time.realtimeSinceStartup;

			//insert a mail
			Transform table = mailScrollView.transform.Find("Table");
			GameObject item = Instantiate(Resources.Load("mailItem") as GameObject); 

		
			//random mail type: image or pure text

			bool isPureText = Random.Range(0f, 1.0f) > 0.5f;

			MailItem m = new MailItem();
			m.marked = false;
			m.title = "updated mail text ...";
			m.isPureText = isPureText;

			m.arriveTime = rTime;
			
			mailBoxList.Add(m);
			mailCount++;
			newIndex++;

			item.name = newIndex.ToString();

			//set parent
			
			item.transform.SetParent(table);

			item = updateMailContent(item, m);
			
			
			item.transform.localScale = Vector3.one;
			
			UIEventListener.Get(item).onClick = OnMailItemClicked;

			gameObject.transform.Find ("ReceiveAmount").GetComponent<UILabel> ().text = mailCount.ToString();

			table.GetComponent<UITable>().repositionNow = true;

		}



	}
	

	void Awake()
	{
		delBtn = transform.Find("DelButton").gameObject;
		mailScrollView = transform.Find("Outline/mailScrollView").gameObject;

		markedBtn = transform.Find("MarkedButton").gameObject;

		progressBar = transform.Find("Outline/ProgressBar").gameObject;


		UIEventListener.Get (delBtn).onClick = OnDelCliked;

		markedList.Release ();

		updateMailTime = 10;

		EventDelegate.Add(progressBar.GetComponent<UIProgressBar> ().onChange, OnProgressChanged);

		InitMailBoxList ();


	}
	

	void InitMailBoxList()
	{
		bool isPureText = true;

		for(int i = 0; i < mailCount; i++)
		{
			isPureText = Random.Range(0f, 1.0f) > 0.5f;

			MailItem m = new MailItem();
			m.marked = false;
			m.title = "mail text " + i + "...";
			m.isPureText = isPureText;
			m.arriveTime = i/50.0f;

			mailBoxList.Add(m);
		}


	}
	

	void Start()
	{

	}

	void OnEnable()
	{
		InitMailList ();

	}


	void InitMailList(int startFrom = 0)
	{

		Transform table = mailScrollView.transform.Find("Table");

		for(int i = startFrom; i < startFrom + viewableCount && i < mailCount; i++)
		{
			GameObject item = Instantiate(Resources.Load("mailItem") as GameObject); 


			//set parent
			item.transform.SetParent(table);



			//random mail type: image or pure text
//			item = RandomMail(item);

			item = updateMailContent(item , mailBoxList[mailCount - i - 1]);


			item.transform.localScale = Vector3.one;

//			item.name = "mail" + (mailCount - i - 1);
			item.name = (mailCount - i - 1).ToString();
//			item.transform.Find("Label").GetComponent<UILabel>().text = "mail"+i;


			UIEventListener.Get(item).onClick = OnMailItemClicked;

		}


		gameObject.transform.Find ("ReceiveAmount").GetComponent<UILabel> ().text = mailCount.ToString();

		//sort 
		table.GetComponent<UITable> ().onCustomSort = CompareMail;

		table.GetComponent<UITable>().repositionNow = true;

	}

	int CompareMail(Transform t1, Transform t2)
	{
		int result = 0;

		result = int.Parse (t2.name) - int.Parse (t1.name);

		return result;
	}



	void OnDelCliked(GameObject go)
	{

		Debug.Log ("On del Clicked.");

		mailCount -= markedList.size;

		UITable table = mailScrollView.transform.Find ("Table").GetComponent<UITable> ();

		foreach(GameObject marked in markedList)
		{
			//to do 
//			marked.SetActive(false);
//			table.RemoveChild(marked.transform);

			//tmp
			Destroy(marked);
		}

		markedList.Release ();
		markedBtn.transform.Find ("Number").GetComponent<UILabel> ().text = "0";
		gameObject.transform.Find ("ReceiveAmount").GetComponent<UILabel> ().text = mailCount.ToString();

		table.repositionNow = true; 

	}


	void OnMailItemClicked(GameObject go)
	{
		if(markedList.Contains(go))
		{
			markedList.Remove(go);
		}
		else
		{
			markedList.Add(go);
		}

		markedBtn.transform.Find ("Number").GetComponent<UILabel> ().text = markedList.size.ToString();

		Debug.Log ("Marked amount: " + markedList.size);
	}
	

	GameObject updateMailContent(GameObject mail, MailItem m)
	{
		mail.transform.Find("Label").GetComponent<UILabel>().text = m.title;
		mail.transform.Find("Time").GetComponent<UILabel>().text = m.arriveTime.ToString();

		if(m.isPureText)
		{
			mail.GetComponent<UISprite>().SetDimensions(1000, 200);
			mail.transform.Find("ImageGroup").gameObject.SetActive(false);
		}
		else
		{
			mail.GetComponent<UISprite>().SetDimensions(1000, 300);
			
			mail.transform.Find("ImageGroup").gameObject.SetActive(true);
			
			int picShow = Random.Range(1, 4);
			
			//show random amount of pic
			for(int j = 0; j< picShow; j++)
			{
				mail.transform.Find("ImageGroup").Find("img"+j).gameObject.SetActive(true);
			}
		}

		return mail;
	}

	void OnProgressChanged()
	{
		float barValue = progressBar.GetComponent<UIProgressBar> ().value;

//		Debug.Log ("bar: " + barValue);


		if(barValue >= 0.99f)
			UpdateNextVisable();

	}

	void UpdateNextVisable()
	{
		InitMailList (shownCount);
		shownCount += viewableCount;
	}
	


}
