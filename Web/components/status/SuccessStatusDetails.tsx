import React from 'react'

import StatusDetails from './StatusDetails'

interface ISuccessStatusDetailsProps {
    title: string,
    successItems: string[]
}

const SuccessStatusDetails = (props: ISuccessStatusDetailsProps) => {
    return (
        <StatusDetails
            title={props.title}
            items={props.successItems}
            itemClassNames={[]}
            backgroundClassNames={['bg-green-50']} />
    )
}

export default SuccessStatusDetails