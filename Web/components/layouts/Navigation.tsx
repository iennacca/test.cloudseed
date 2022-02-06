import React from 'react'
import Link from 'next/link'

type NavigationProps = {
    homeLink: NavigationLink,
    navigationLinks?: NavigationLink[]
}

export type NavigationLink = {
    name: string,
    href: string
}

const Navigation = (props: NavigationProps) => {
    const getNavigationLinksMarkup = (navigationLinks?: NavigationLink[]) => {
        if(!navigationLinks) {
            return <></>
        }

        return (
            <React.Fragment>
                {
                    navigationLinks.map(link => {
                        return (
                            <Link href={link.href}
                                key={link.name}>
                                <a className="block mt-4 lg:inline-block lg:mt-0 hover:text-hamred mr-6 lg:flex">
                                    {link.name}
                                </a>
                            </Link>
                        )
                    })
                }
            </React.Fragment>
        )
    }

    return <nav className="flex items-center justify-between flex-wrap p-6 text-black">
        <div className="flex flex-shrink-0 text-black mr-6">
            <Link href={ props.homeLink.href }>
                <a className="font-semibold text-xl tracking-tight hover:text-hamred">
                    { props.homeLink.name }
                </a>
            </Link>
        </div>
        <div className='flex items-right'>
            {
                getNavigationLinksMarkup(props.navigationLinks)
            }
        </div>
      </nav>
}

export default Navigation;