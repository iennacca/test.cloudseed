import React from 'react'

import StatusDetails from './StatusDetails'

interface IErrorDetailsProps {
    title: string,
    errorItems: string[]
}

const ErrorDetails = (props: IErrorDetailsProps) => {
    console.error(props.errorItems);

    return (
        <StatusDetails
            title={props.title}
            items={props.errorItems}
            itemClassNames={[]}
            backgroundClassNames={['bg-red-50']} />
    )
}

export default ErrorDetails