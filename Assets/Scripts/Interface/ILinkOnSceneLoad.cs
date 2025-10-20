using UnityEngine;

// 전역 오브젝트 중 씬 로드 시점에 다시 링크가 필요한 오브젝트가 있을 때 구현
public interface ILinkOnSceneLoad
{
    // 씬 전환 시 관련 오브젝트 링크
    public void LinkObjectsOnSceneLoad();
}