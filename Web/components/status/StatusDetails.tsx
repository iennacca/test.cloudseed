import React from 'react'

interface IStatusDetailsProps {
    title: string,
    items: string[],
    itemClassNames: string[],
    backgroundClassNames: string[]
}

const StatusDetails = (props: IStatusDetailsProps) => {
    const getItemTsx = (items: string[], itemClassNames: string[]) => {
        return items.length === 0
            ? <></>
            : <div className={`mt-2 text-sm ${itemClassNames.join(' ')}`}>
                <ul role="list" className="list-disc pl-5 space-y-1">   
                    { 
                        items.map( (item, index) => {
                            return <li key={index}>{item}</li>
                        })
                    }
                </ul>
            </div>
    }

    return (
        <div className={`rounded-md p-4 ${props.backgroundClassNames.join(' ')}`}>
            <div className="ml-3">
            <h3 className={`text-sm font-medium`}>{props.title}</h3>
            {
                getItemTsx(props.items, props.itemClassNames)
            }
            </div>
        </div>
    )
}

export default StatusDetails