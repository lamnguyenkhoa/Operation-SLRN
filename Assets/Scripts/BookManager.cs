using UnityEngine;

public class BookManager : MonoBehaviour
{
    private int leftPageIndex = 0;
    private int rightPageIndex = 1;
    public GameObject[] allPages; // should be 12, or at least even number;

    void Update()
    {
        if (Input.GetButtonDown("Horizontal"))
        {
            float moveHorizontal = Input.GetAxis("Horizontal");
            if (moveHorizontal > 0)
            {
                moveHorizontal = 0;
                MoveNextPages();
            }
            else if (moveHorizontal < 0)
            {
                moveHorizontal = 0;
                MovePreviousPages();
            }
        }
    }

    public void MoveNextPages()
    {
        if (leftPageIndex + 2 <= allPages.Length - 1)
        {
            // Not end of book yet
            allPages[leftPageIndex].SetActive(false);
            allPages[rightPageIndex].SetActive(false);

            leftPageIndex = leftPageIndex + 2;
            rightPageIndex = rightPageIndex + 2;

            allPages[leftPageIndex].SetActive(true);
            allPages[rightPageIndex].SetActive(true);

            GameManager.instance.PlayBookFlipping();
        }
    }

    public void MovePreviousPages()
    {
        if (leftPageIndex - 2 >= 0)
        {
            // Not end of book yet
            allPages[leftPageIndex].SetActive(false);
            allPages[rightPageIndex].SetActive(false);

            leftPageIndex = leftPageIndex - 2;
            rightPageIndex = rightPageIndex - 2;

            allPages[leftPageIndex].SetActive(true);
            allPages[rightPageIndex].SetActive(true);

            GameManager.instance.PlayBookFlipping();
        }
    }
}
