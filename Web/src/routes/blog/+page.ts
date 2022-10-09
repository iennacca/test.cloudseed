import type { BlogPostListItem } from "./BlogTypes"

const getFileName = (fullPath : string) => {
  let filepathSplitBySlash = fullPath
    .split("/")
  let fileNameWithEnding = filepathSplitBySlash[filepathSplitBySlash.length - 1]
  
  let indexOfPeriod = fileNameWithEnding.indexOf(".")
  let fileNameWithoutEnding = fileNameWithEnding
    .substring(
      0,
      indexOfPeriod >= 0
      ? indexOfPeriod
      : null
    )
  return fileNameWithoutEnding
}

export async function load({ params }){
    /* @vite-ignore */
    const allPostFiles = import.meta.glob(`./posts/*.svx`)
    const iterablePostFiles = Object.entries(allPostFiles)

    const allPosts = await Promise.all(
      iterablePostFiles.map(async ([path, resolver]) => {
        // @ts-ignore
        const { metadata } = await resolver()
        const postPath = getFileName(path)
  
        return {
          meta: metadata,
          path: "/blog/" + postPath,
        }
      })
    )

    const allPostListItems = allPosts.map(p => {
      return {
        path: p.path,
        title : p.meta.title,
        date : p.meta.date,
        tags: []
      } as BlogPostListItem
    })

    return {
      allPosts: allPostListItems
    }
  }