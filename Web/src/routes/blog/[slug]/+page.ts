import type { PostData } from "./PostData"

export async function load({ params }){
    const post = await import(
        /* @vite-ignore */
        `../posts/${params.slug}.svx`)
    const { title, date } = post.metadata
    const content = post.default
  
    return {
      postData: {
        content: content,
        title: title,
        date: date,
      } as PostData
    }
  }