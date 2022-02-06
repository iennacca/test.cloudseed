const stackItems: {
    name: string,
    logoUrl: string,
    projectUrl: string
}[] = [
  {
    name: ".NET",
    logoUrl: "/logos/dotnet-logo.png",
    projectUrl: "https://dotnet.microsoft.com/"
  },
  {
    name: "Entity Framework",
    logoUrl: "/logos/efcore-logo.png",
    projectUrl: "https://docs.microsoft.com/en-us/ef/"
  },
  {
    name: "Postgres",
    logoUrl: "/logos/postgres-logo.png",
    projectUrl: "https://www.postgresql.org/"
  },
  {
    name: "NextJS",
    logoUrl: "/logos/nextjs-logo.png",
    projectUrl: "https://nextjs.org/"
  },
  {
    name: "npm",
    logoUrl: "/logos/npm-logo.png",
    projectUrl: "https://www.npmjs.com/"
  },
  {
    name: "Docker",
    logoUrl: "/logos/docker-logo.png",
    projectUrl: "https://www.docker.com/"
  }
]

export default function TechStack() {
  return (
    <div className="py-12 bg-white" id="tech_stack">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="lg:text-center">
          <h2 className="text-base text-indigo-600 font-semibold tracking-wide uppercase">Tech Stack</h2>
          <p className="mt-2 text-3xl leading-8 font-extrabold tracking-tight text-gray-900 sm:text-4xl">
            Familiar, Battle-Tested Tools
          </p>
        </div>
        <div className="flex flex-wrap py-6">
          {
            stackItems.map((item) => (
              <div className="p-1 w-20 h-20"
                key={item.logoUrl} >
                  <a href={item.projectUrl}>
                    <img 
                      src={item.logoUrl}
                      className="object-contain h-32" />
                  </a>
              </div>
            ))
          }
        </div>
      </div>
    </div>
  )
}